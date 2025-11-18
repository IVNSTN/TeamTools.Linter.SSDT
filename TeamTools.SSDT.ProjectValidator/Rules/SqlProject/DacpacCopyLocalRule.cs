using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Rules.SqlProject
{
    [DataType("SQL")]
    [RuleIdentity("SSDT0005", "DACPAC_COPY_LOCAL")]
    public sealed class DacpacCopyLocalRule : BaseDacpacRule
    {
        public DacpacCopyLocalRule() : base()
        {
        }

        protected override string GetNodeSelectExpression()
        {
            return $"/{SsdtNamespace}:Project/{SsdtNamespace}:ItemGroup/{SsdtNamespace}:ArtifactReference[substring(@Include, string-length(@Include)-{DacpacExtension.Length}+1) = '{DacpacExtension}' and ./{SsdtNamespace}:Private != 'False']";
        }
    }
}
