using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0008", "DATASOURCE_NAME_ALLOWED_SYMBOLS")]
    public sealed class DatasourceReferenceNameAlphanumericRule : BaseDataToolsXmlNodesRule
    {
        private const string ReportsNamespace = "rdef";
        private static readonly string NodeSelector = $"//{ReportsNamespace}:DataSources/{ReportsNamespace}:DataSource";
        private static readonly Regex DisallowedSymbols = new Regex(@"[^\w.-]", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public DatasourceReferenceNameAlphanumericRule() : base()
        {
            DatasourceNamespace = ReportsNamespace;
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            foreach (var ds in nodes)
            {
                string datasourceName = ds.Attribute("Name")?.Value;
                if (!string.IsNullOrEmpty(datasourceName) && DisallowedSymbols.IsMatch(datasourceName))
                {
                    HandleNodeError(ds, datasourceName);
                }
            }
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
