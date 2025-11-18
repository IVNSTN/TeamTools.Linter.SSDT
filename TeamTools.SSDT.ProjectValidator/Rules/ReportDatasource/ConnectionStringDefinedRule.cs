using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.ReportDatasource
{
    [DataType("ReportDatasource")]
    [RuleIdentity("RDS0003", "CONNECTION_STRING_UNDEFINED")]
    public sealed class ConnectionStringDefinedRule : BaseDataToolsXmlNodesRule
    {
        private static readonly string NodeSelector = @"//ConnectionProperties/ConnectString[normalize-space(text()) != '']";

        public ConnectionStringDefinedRule() : base()
        {
            DatasourceNamespace = "rds";
        }

        protected override void DoWhenNotFound()
        {
            HandleFileError();
        }

        protected override string GetNodeSelectExpression()
        {
            return NodeSelector;
        }
    }
}
