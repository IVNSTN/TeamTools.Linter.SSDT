using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.ReportDatasource
{
    [DataType("ReportDatasource")]
    [RuleIdentity("RDS0007", "DATASOURCE_NAME_TEMPLATE")]
    public sealed class DatasourceNameTemplateRule : BaseDataToolsXmlNodesRule
    {
        private static readonly string NodeSelector = "//RptDataSource";
        private static readonly Regex NamePattern = new Regex(@"^(?<server>[\w]+)\.(?<db>[\w]+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public DatasourceNameTemplateRule() : base()
        {
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            foreach (var ds in nodes)
            {
                string datasourceName = ds.Attribute("Name")?.Value;
                if (!string.IsNullOrEmpty(datasourceName) && !NamePattern.IsMatch(datasourceName))
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
