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
    [RuleIdentity("RDL0010", "UNUSED_DATASOURCE")]
    public sealed class DatasourceNeverUserRule : BaseDataToolsRule
    {
        private const string ReportsNamespace = "rdef";

        public DatasourceNeverUserRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo rdl)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(ReportsNamespace, rdl.FileDom.Root.GetDefaultNamespace().NamespaceName);

            var datasources = ExtractDatasources(rdl.FileDom, ns);
            if (!datasources.Any())
            {
                return;
            }

            var refs = ExtractLinksToDatasources(rdl.FileDom, ns);

            foreach (var ds in datasources.Keys)
            {
                if (!refs.Contains(ds))
                {
                    HandleNodeError(datasources[ds], ds);
                }
            }
        }

        private static ICollection<string> ExtractLinksToDatasources(XDocument doc, XmlNamespaceManager ns)
        {
            var nodes = doc.XPathSelectElements($"//{ReportsNamespace}:DataSourceName", ns);
            var result = new HashSet<string>(
                nodes
                    .Select(nd => nd.Value)
                    .Distinct(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);

            return result;
        }

        private static IDictionary<string, XElement> ExtractDatasources(XDocument doc, XmlNamespaceManager ns)
        {
            var nodes = doc.XPathSelectElements($"//{ReportsNamespace}:DataSources/{ReportsNamespace}:DataSource", ns);
            var result = nodes
                .Where(nd => nd.Attribute("Name") != null)
                .GroupBy(nd => nd.Attribute("Name").Value)
                .ToDictionary(
                    grp => grp.Key,
                    grp => grp.First(),
                    StringComparer.OrdinalIgnoreCase);

            return result;
        }
    }
}
