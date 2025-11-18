using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Routines;

namespace TeamTools.SSDT.ProjectValidator.Rules.ReportDatasource
{
    [DataType("ReportDatasource")]
    [RuleIdentity("RDS0001", "SERVER_LINK_FULLY_QUALIFIED")]
    public sealed class ServerNameFullyQualifiedRule : BaseDataToolsXmlNodesRule
    {
        private static readonly string NodeSelector = "//ConnectionProperties/ConnectString";
        private static readonly string ConnectionStringKey = "Data Source";
        private static readonly Regex NamePattern = new Regex(@"^(?<server>[\w-]+)\.(?<domain>[\w-]+)\.(?<zone>[\w]+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public ServerNameFullyQualifiedRule() : base()
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
                    continue;
                }

                if (string.IsNullOrEmpty(cs[ConnectionStringKey])
                || !NamePattern.IsMatch(cs[ConnectionStringKey]))
                {
                    HandleNodeError(connection, cs[ConnectionStringKey]);
                }
            }
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
