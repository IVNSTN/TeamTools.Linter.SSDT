using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Routines;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [DataType("REPORT")]
    [RuleIdentity("SSDT0009", "PROJECT_INCLUDE_ABSOLUTE_PATH")]
    public sealed class AbsolutePathRule : BaseDataToolsRule
    {
        private static readonly string IncludeSelectExpression = $"/{SsdtNamespace}:Project/{SsdtNamespace}:ItemGroup/*[@Include != '']";
        private static readonly string OutputSelectExpression = $"/{SsdtNamespace}:Project/{SsdtNamespace}:PropertyGroup/{SsdtNamespace}:OutputPath";
        private static readonly string ScriptNameSelectExpression = $"/{SsdtNamespace}:Project/{SsdtNamespace}:PropertyGroup/{SsdtNamespace}:BuildScriptName";
        private static readonly string ImportSelectExpression = $"//{SsdtNamespace}:Import[@Project != '']";
        private static readonly string RootPathSelectExpression = $"//{SsdtNamespace}:RootPath";

        public AbsolutePathRule() : base()
        {
        }

        protected override void DoValidate(DataToolsFileInfo fileInfo)
        {
            var otherPaths = ExtractPathMentions(fileInfo.FileDom);
            ValidateItems(otherPaths);
        }

        private static IEnumerable<Tuple<string, IXmlLineInfo>> ExtractPathMentions(XDocument doc)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(SsdtNamespace, doc.Root.GetDefaultNamespace().NamespaceName);

            return NetExtensions.CombineEnumerables<Tuple<string, IXmlLineInfo>>(
                ExtractByExpression(doc, ns, IncludeSelectExpression, "Include"),
                ExtractByExpression(doc, ns, OutputSelectExpression),
                ExtractByExpression(doc, ns, ScriptNameSelectExpression),
                ExtractByExpression(doc, ns, ImportSelectExpression, "Project"),
                ExtractByExpression(doc, ns, RootPathSelectExpression));
        }

        private static IEnumerable<Tuple<string, IXmlLineInfo>> ExtractByExpression(XDocument doc, XmlNamespaceManager ns, string searchExpression)
        {
            foreach (var node in doc.XPathSelectElements(searchExpression, ns))
            {
                yield return new Tuple<string, IXmlLineInfo>(node.Value, node is IXmlLineInfo li ? li : null);
            }
        }

        private static IEnumerable<Tuple<string, IXmlLineInfo>> ExtractByExpression(XDocument doc, XmlNamespaceManager ns, string searchExpression, string attrName)
        {
            foreach (var node in doc.XPathSelectElements(searchExpression, ns))
            {
                yield return new Tuple<string, IXmlLineInfo>(node.Attribute(attrName).Value, node is IXmlLineInfo li ? li : null);
            }
        }

        private void ValidateItems(IEnumerable<Tuple<string, IXmlLineInfo>> pathItems)
        {
            foreach (var pathItem in pathItems)
            {
                string includePath = pathItem.Item1;
                if (!string.IsNullOrWhiteSpace(includePath) && Path.IsPathRooted(includePath))
                {
                    HandleLineError(pathItem.Item2.LineNumber, pathItem.Item2.LinePosition, includePath);
                }
            }
        }
    }
}
