using System.Collections.Generic;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.ReportDatasource
{
    [DataType("ReportDatasource")]
    [RuleIdentity("RDS0004", "DATASOURCE_NAME_LENGTH")]
    public sealed class DatasourceNameMinLengthRule : BaseDataToolsXmlNodesRule
    {
        private const int MinLength = 3;
        private static readonly string NodeSelector = "//RptDataSource";

        public DatasourceNameMinLengthRule() : base()
        {
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            foreach (var ds in nodes)
            {
                string datasourceName = ds.Attribute("Name")?.Value.Trim();
                if (string.IsNullOrEmpty(datasourceName) || datasourceName.Length < MinLength)
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
