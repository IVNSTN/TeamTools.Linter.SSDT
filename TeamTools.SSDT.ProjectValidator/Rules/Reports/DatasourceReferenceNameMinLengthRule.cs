using System.Collections.Generic;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0007", "DATASOURCE_NAME_LENGTH")]
    public sealed class DatasourceReferenceNameMinLengthRule : BaseDataToolsXmlNodesRule
    {
        private const string ReportsNamespace = "rdef";
        private const int MinLength = 2;
        private static readonly string NodeSelector = $"//{ReportsNamespace}:DataSources/{ReportsNamespace}:DataSource";

        public DatasourceReferenceNameMinLengthRule() : base()
        {
            DatasourceNamespace = ReportsNamespace;
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
