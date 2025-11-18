using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.ReportDatasource
{
    [DataType("ReportDatasource")]
    [RuleIdentity("RDS0005", "DATASOURCE_NAME_ALLOWED_SYMBOLS")]
    public sealed class DatasourceNameAlphanumericRule : BaseDataToolsXmlNodesRule
    {
        private static readonly string NodeSelector = "//RptDataSource";
        private static readonly Regex DisallowedSymbols = new Regex(@"[^\w.-]", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public DatasourceNameAlphanumericRule() : base()
        {
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
