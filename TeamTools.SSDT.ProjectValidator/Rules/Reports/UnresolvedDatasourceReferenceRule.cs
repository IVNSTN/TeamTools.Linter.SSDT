using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0011", "UNRESOLVED_DATASOURCE_REFERENCE")]
    public sealed class UnresolvedDatasourceReferenceRule : BaseDataToolsRule
    {
        private const string ReportsNamespace = "rdef";
        private const string MsgTemplate = "dataset {0} references '{1}' which does not exist";

        public UnresolvedDatasourceReferenceRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo rdl)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(ReportsNamespace, rdl.FileDom.Root.GetDefaultNamespace().NamespaceName);

            var refs = ExtractLinksToDatasources(rdl.FileDom, ns);
            if (!refs.Any())
            {
                return;
            }

            var datasources = ExtractDatasources(rdl.FileDom, ns);

            foreach (var ds in refs.Keys)
            {
                string datasourceName = refs[ds].Value;
                if (!string.IsNullOrEmpty(datasourceName) && !datasources.Contains(datasourceName))
                {
                    HandleNodeError(refs[ds].Key, string.Format(MsgTemplate, ds, datasourceName));
                }
            }
        }

        // key - dataset name, value = dataset node and name of referenced datasource
        private static IDictionary<string, KeyValuePair<XElement, string>> ExtractLinksToDatasources(XDocument doc, XmlNamespaceManager ns)
        {
            var nodes = doc.XPathSelectElements($"//{ReportsNamespace}:DataSets/{ReportsNamespace}:DataSet[.//{ReportsNamespace}:DataSourceName]", ns);
            var result = nodes
                .Where(nd => nd.Attribute("Name") != null)
                .GroupBy(nd => nd.Attribute("Name").Value)
                .ToDictionary(
                    grp => grp.Key,
                    grp => new KeyValuePair<XElement, string>(
                        grp.First(),
                        grp.First().XPathSelectElements($".//{ReportsNamespace}:DataSourceName", ns).FirstOrDefault()?.Value),
                    StringComparer.OrdinalIgnoreCase);

            return result;
        }

        private static ICollection<string> ExtractDatasources(XDocument doc, XmlNamespaceManager ns)
        {
            var nodes = doc.XPathSelectElements($"//{ReportsNamespace}:DataSources/{ReportsNamespace}:DataSource", ns);
            var result = new HashSet<string>(
                nodes
                    .Where(nd => nd.Attribute("Name") != null)
                    .Select(nd => nd.Attribute("Name").Value)
                    .Distinct(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);

            return result;
        }
    }
}
