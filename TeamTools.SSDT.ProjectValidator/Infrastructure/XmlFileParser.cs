using System.IO;
using System.Xml.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Infrastructure
{
    internal class XmlFileParser : IFileParser<DataToolsFileInfo>
    {
        public DataToolsFileInfo Parse(ILintingContext context)
        {
            XDocument fileDom = XDocument.Load(context.FileContents, LoadOptions.SetLineInfo);

            if (fileDom is null)
            {
                throw new InvalidDataException("Could not build XML DOM");
            }

            return new DataToolsFileInfo(context.FilePath, fileDom);
        }
    }
}
