using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidator.Interfaces
{
    public interface IDataToolsRule : ILinterRule
    {
        void Validate(DataToolsFileInfo fileInfo);

        string GetSupportedDataType();
    }
}
