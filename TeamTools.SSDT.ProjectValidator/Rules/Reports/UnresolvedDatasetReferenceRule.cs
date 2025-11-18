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
    [RuleIdentity("RDL0013", "UNRESOLVED_DATASET_REFERENCE")]
    public sealed class UnresolvedDatasetReferenceRule : BaseDataToolsRule
    {
        private const string ReportsNamespace = "rdef";
        private const string MsgTemplate = "{0} references '{1}' which does not exist";

        public UnresolvedDatasetReferenceRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo rdl)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(ReportsNamespace, rdl.FileDom.Root.GetDefaultNamespace().NamespaceName);

            var refs = ExtractLinksToDatasets(rdl.FileDom, ns);
            if (!refs.Any())
            {
                return;
            }

            var datasets = ExtractDatasources(rdl.FileDom, ns);

            foreach (var ds in refs.Keys)
            {
                string datasourceName = refs[ds].Value;
                if (!string.IsNullOrEmpty(datasourceName) && !datasets.Contains(datasourceName))
                {
                    HandleNodeError(refs[ds].Key, string.Format(MsgTemplate, ds, datasourceName));
                }
            }
        }

        // key - ref name, value = ref node and name of referenced dataset
        private static IDictionary<string, KeyValuePair<XElement, string>> ExtractLinksToDatasets(XDocument doc, XmlNamespaceManager ns)
        {
            var paramNodes = doc.XPathSelectElements($"//{ReportsNamespace}:ReportParameter[.//{ReportsNamespace}:DataSetName]", ns)
                 .GroupBy(nd => "ReportParameter " + nd.Attribute("Name")?.Value ?? "noname");

            var tablixNodes = doc.XPathSelectElements($"//{ReportsNamespace}:Tablix[.//{ReportsNamespace}:DataSetName]", ns)
                 .GroupBy(nd => "Tablix " + nd.Attribute("Name")?.Value ?? "noname");

            var result = paramNodes
                .Union(tablixNodes)
                .ToDictionary(
                    grp => grp.Key,
                    grp => new KeyValuePair<XElement, string>(
                        grp.First(),
                        grp.First().XPathSelectElements($".//{ReportsNamespace}:DataSetName", ns).FirstOrDefault()?.Value),
                    StringComparer.OrdinalIgnoreCase);

            return result;
        }

        private static ICollection<string> ExtractDatasources(XDocument doc, XmlNamespaceManager ns)
        {
            var nodes = doc.XPathSelectElements($"//{ReportsNamespace}:DataSets/{ReportsNamespace}:DataSet", ns);
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
