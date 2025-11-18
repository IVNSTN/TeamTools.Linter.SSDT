using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [RuleIdentity("SSDT0003", "DACPAC_WARNING_SUPPRESSED")]
    public sealed class DacpacWarningSuppressedRule : BaseDacpacRule
    {
        public DacpacWarningSuppressedRule() : base()
        {
        }

        protected override string GetNodeSelectExpression()
        {
            return $"/{SsdtNamespace}:Project/{SsdtNamespace}:ItemGroup/{SsdtNamespace}:ArtifactReference[substring(@Include, string-length(@Include)-{DacpacExtension.Length}+1) = '{DacpacExtension}' and not ({SsdtNamespace}:SuppressMissingDependenciesErrors = 'True')]";
        }
    }
}
