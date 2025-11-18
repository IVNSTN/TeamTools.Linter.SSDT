using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0012", "UNUSED_DATASET")]
    public sealed class DatasetNeverUsedRule : BaseDataToolsRule
    {
        private const string ReportsNamespace = "rdef";
        private static readonly Regex PossibleLinks = new Regex(@"[(\s,]+""(?<refName>[\w]+)""[)\s,]+", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.Compiled);

        public DatasetNeverUsedRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo rdl)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(ReportsNamespace, rdl.FileDom.Root.GetDefaultNamespace().NamespaceName);

            var datasets = ExtractDatasets(rdl.FileDom, ns);
            if (!datasets.Any())
            {
                return;
            }

            var refs = ExtractLinksToDatasets(rdl.FileDom, ns);

            foreach (var ds in datasets.Keys)
            {
                if (!refs.Contains(ds))
                {
                    HandleNodeError(datasets[ds], ds);
                }
            }
        }

        private static ICollection<string> ExtractLinksToDatasets(XDocument doc, XmlNamespaceManager ns)
        {
            var directLinks = doc.XPathSelectElements($"//{ReportsNamespace}:DataSetName", ns)
                .Select(nd => nd.Value)
                .Where(refName => !string.IsNullOrEmpty(refName));

            var computations = doc.XPathSelectElements($"//{ReportsNamespace}:Value", ns)
                .SelectMany(nd => PossibleLinks.Matches(nd.Value).Cast<Match>())
                .Where(m => m.Captures.Count > 0)
                .Select(m => m.Groups["refName"].Value)
                .Where(refName => !string.IsNullOrEmpty(refName));

            var tooltips = doc.XPathSelectElements($"//{ReportsNamespace}:ToolTip", ns)
                .SelectMany(nd => PossibleLinks.Matches(nd.Value).Cast<Match>())
                .Where(m => m.Captures.Count > 0)
                .Select(m => m.Groups["refName"].Value)
                .Where(refName => !string.IsNullOrEmpty(refName));

            var isHidden = doc.XPathSelectElements($"//{ReportsNamespace}:Hidden", ns)
                .SelectMany(nd => PossibleLinks.Matches(nd.Value).Cast<Match>())
                .Where(m => m.Captures.Count > 0)
                .Select(m => m.Groups["refName"].Value)
                .Where(refName => !string.IsNullOrEmpty(refName));

            var result = new HashSet<string>(
                directLinks
                .Union(computations)
                .Union(tooltips)
                .Union(isHidden)
                .Distinct(),
                StringComparer.OrdinalIgnoreCase);

            return result;
        }

        private static IDictionary<string, XElement> ExtractDatasets(XDocument doc, XmlNamespaceManager ns)
        {
            var nodes = doc.XPathSelectElements($"//{ReportsNamespace}:DataSets/{ReportsNamespace}:DataSet", ns);
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
