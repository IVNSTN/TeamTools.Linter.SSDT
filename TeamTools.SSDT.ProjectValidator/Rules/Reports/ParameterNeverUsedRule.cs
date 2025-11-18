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
    [RuleIdentity("RDL0014", "UNUSED_PARAMETER")]
    public sealed class ParameterNeverUsedRule : BaseDataToolsRule
    {
        private const string ReportsNamespace = "rdef";
        private static readonly Regex ParameterReference = new Regex(@"Parameters[!](?<param>[\w]+)[.]", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public ParameterNeverUsedRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo rdl)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(ReportsNamespace, rdl.FileDom.Root.GetDefaultNamespace().NamespaceName);

            var parameters = ExtractParameters(rdl.FileDom, ns);
            if (!parameters.Any())
            {
                return;
            }

            var refs = ExtractLinksToParameters(rdl.FileDom, ns);

            foreach (var ds in parameters.Keys)
            {
                if (!refs.Contains(ds))
                {
                    HandleNodeError(parameters[ds], ds);
                }
            }
        }

        private static ICollection<string> ExtractLinksToParameters(XDocument doc, XmlNamespaceManager ns)
        {
            var directLinks = doc.XPathSelectElements($"//{ReportsNamespace}:ParameterName", ns)
                    .Select(nd => nd.Value);

            var formulas = doc.XPathSelectElements($"//{ReportsNamespace}:Value[contains(text(), 'Parameters!')]", ns)
                            .SelectMany(nd => ParameterReference.Matches(nd.Value).Cast<Match>())
                            .Where(m => m.Captures.Count > 0)
                            .Select(m => m.Groups["param"].Value);

            var tooltips = doc.XPathSelectElements($"//{ReportsNamespace}:ToolTip[contains(text(), 'Parameters!')]", ns)
                            .SelectMany(nd => ParameterReference.Matches(nd.Value).Cast<Match>())
                            .Where(m => m.Captures.Count > 0)
                            .Select(m => m.Groups["param"].Value);

            var isHidden = doc.XPathSelectElements($"//{ReportsNamespace}:Hidden[contains(text(), 'Parameters!')]", ns)
                            .SelectMany(nd => ParameterReference.Matches(nd.Value).Cast<Match>())
                            .Where(m => m.Captures.Count > 0)
                            .Select(m => m.Groups["param"].Value);

            var result = new HashSet<string>(
                directLinks
                .Union(formulas)
                .Union(tooltips)
                .Union(isHidden)
                .Distinct(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);

            return result;
        }

        private static IDictionary<string, XElement> ExtractParameters(XDocument doc, XmlNamespaceManager ns)
        {
            var nodes = doc.XPathSelectElements($"//{ReportsNamespace}:ReportParameters/{ReportsNamespace}:ReportParameter", ns);
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
