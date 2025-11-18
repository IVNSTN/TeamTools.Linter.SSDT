using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [DataType("REPORT")]
    [RuleIdentity("SSDT0001", "PROJECT_DUP_INCLUDE")]
    public class DuplicateIncludeRule : BaseDataToolsRule
    {
        private const string IncludeAttributeName = "Include";
        private static readonly string IncludeSelectExpression = $"/{SsdtNamespace}:Project/{SsdtNamespace}:ItemGroup/*[@{IncludeAttributeName} != '']";

        public DuplicateIncludeRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo fileInfo)
            => ValidateIncludedItems(ExtractIncludeDirectives(fileInfo.FileDom));

        protected virtual void ValidateIncludedItems(IDictionary<string, List<int>> includedItems)
        {
            foreach (var include in includedItems)
            {
                if (include.Value.Count <= 1)
                {
                    // no dups
                    continue;
                }

                string fileName = Path.GetFileName(include.Key);
                string lines = string.Join(", ", include.Value.OrderBy(x => x).Select(x => x.ToString()));

                HandleLineError(include.Value[0], 0, string.Format("{0} at lines {1}", fileName, lines));
            }
        }

        private static Dictionary<string, List<int>> ExtractIncludeDirectives(XDocument doc)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(SsdtNamespace, doc.Root.GetDefaultNamespace().NamespaceName);
            var nodes = doc.XPathSelectElements(IncludeSelectExpression, ns);
            var includeAttr = XName.Get(IncludeAttributeName, ns.DefaultNamespace);

            return ExtractIncludeItems(nodes, includeAttr);
        }

        private static Dictionary<string, List<int>> ExtractIncludeItems(IEnumerable<XElement> nodes, XName includeAttr)
        {
            var includedItems = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var dups = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

            foreach (XElement node in nodes)
            {
                string includePath = node.Attribute(includeAttr).Value;

                if (string.IsNullOrEmpty(includePath))
                {
                    continue;
                }

                int lineNumber;
                if (node is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
                {
                    lineNumber = lineInfo.LineNumber;
                }
                else
                {
                    // line info is missing but element still exists and we need to record it
                    lineNumber = 1;
                }

                if (!includedItems.TryGetValue(includePath, out var priorPosition))
                {
                    includedItems.Add(includePath, lineNumber);
                }
                else
                {
                    if (!dups.TryGetValue(includePath, out var includePositions))
                    {
                        includePositions = new List<int> { priorPosition };
                        dups.Add(includePath, includePositions);
                    }

                    includePositions.Add(lineNumber);
                }
            }

            return dups;
        }
    }
}
