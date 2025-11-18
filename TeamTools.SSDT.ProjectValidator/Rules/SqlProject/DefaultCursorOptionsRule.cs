using System;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Properties;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [RuleIdentity("SSDT0008", "SQL_PROJECT_CURSOR_OPTIONS")]
    public sealed class DefaultCursorOptionsRule : BaseDataToolsRule
    {
        private const string ExpectedCursorScope = "LOCAL";
        private static readonly string Expr = $"/{SsdtNamespace}:Project/{SsdtNamespace}:PropertyGroup/{SsdtNamespace}:DefaultCursor";

        public DefaultCursorOptionsRule() : base()
        {
        }

        private static string MissingOptionMessage => Strings.ViolationDetails_DefaultCursorOptionsRule_missing_definition;

        private static string RedundantOptionMessage => Strings.ViolationDetails_DefaultCursorOptionsRule_redundant_definition;

        protected override void DoValidate(DataToolsFileInfo fileInfo)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace(SsdtNamespace, fileInfo.FileDom.Root.GetDefaultNamespace().NamespaceName);
            var nodes = fileInfo.FileDom.XPathSelectElements(Expr, ns).ToArray();

            if (nodes is null || nodes.Length == 0)
            {
                HandleFileError(MissingOptionMessage);
                return;
            }

            if (nodes.Length > 1)
            {
                HandleFileError(RedundantOptionMessage);
                return;
            }

            var cursorOption = nodes[0];
            if (cursorOption.Value.Equals(ExpectedCursorScope, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            HandleNodeError(cursorOption, cursorOption.Value);
        }
    }
}
