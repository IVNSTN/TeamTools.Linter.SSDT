using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Routines;

namespace TeamTools.SSDT.ProjectValidator.Rules.ReportDatasource
{
    [DataType("ReportDatasource")]
    [RuleIdentity("RDS0002", "INTEGRATED_SECURITY_VALUE")]
    public sealed class IntegratedSecurityInDatasourceRule : BaseDataToolsXmlNodesRule
    {
        private static readonly string NodeSelector = "//ConnectionProperties/ConnectString";
        private static readonly string ConnectionStringKey = "Integrated Security";

        public IntegratedSecurityInDatasourceRule() : base()
        {
        }

        protected override void DoWhenFound(IEnumerable<XElement> nodes)
        {
            foreach (var connection in nodes)
            {
                var cs = ConnectionStringParser.Parse(connection.Value?.Trim());
                if (cs is null)
                {
                    continue;
                }

                if (!cs.ContainsKey(ConnectionStringKey))
                {
                    HandleNodeError(connection, "undefined");
                    continue;
                }

                if (!string.Equals(cs[ConnectionStringKey], "SSPI", StringComparison.OrdinalIgnoreCase))
                {
                    HandleNodeError(connection, cs[ConnectionStringKey] + " instead of SSPI");
                }
            }
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
