using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.Reports
{
    [DataType("ReportDesign")]
    [RuleIdentity("RDL0002", "INTEGRATED_SECURITY_OFF")]
    public sealed class IntegratedSecurityInReportRule : BaseDataToolsXmlNodesRule
    {
        private const string ReportsNamespace = "rdef";
        private static readonly string NodeSelector = $"//{ReportsNamespace}:IntegratedSecurity[normalize-space(text()) != 'None']";

        public IntegratedSecurityInReportRule() : base()
        {
            DatasourceNamespace = ReportsNamespace;
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            HandleNodeError(nodes.First(), nodes.First().Value + " instead of None");
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
