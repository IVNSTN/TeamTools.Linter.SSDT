using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0003", "CONNECTION_STRING_ILLEGAL")]
    public sealed class ConnectionStringInReportRule : BaseDataToolsXmlNodesRule
    {
        private const string ReportsNamespace = "rdef";
        private static readonly string NodeSelector = $"//{ReportsNamespace}:ConnectString";

        public ConnectionStringInReportRule() : base()
        {
            DatasourceNamespace = ReportsNamespace;
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            HandleNodeError(nodes.First(), "ConnectString");
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
