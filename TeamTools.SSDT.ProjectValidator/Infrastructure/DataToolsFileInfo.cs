using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace TeamTools.SSDT.ProjectValidator.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class DataToolsFileInfo
    {
        public DataToolsFileInfo(string filePath, XDocument doc)
        {
            this.FilePath = filePath;
            this.FileDom = doc;
        }

        public string FilePath { get; }

        public XDocument FileDom { get; }
    }
}
