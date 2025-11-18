using System.Collections.Generic;

namespace TeamTools.SSDT.ProjectValidator.Interfaces
{
    public interface IFolderScanningRule
    {
        void InjectFolderScanner(IFolderScanner scanner);
    }

    public interface IFolderScanner
    {
        IEnumerable<string> ListFiles(bool recursive = false);

        IEnumerable<string> ListFolders(bool recursive = false);
    }
}
