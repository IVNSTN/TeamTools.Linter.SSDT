using System.IO;
using System.Xml.Linq;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Interfaces;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    public class MockLinter
    {
        public void Lint(string scriptPath, IDataToolsRule rule)
        {
            using var reader = new StreamReader(scriptPath);
            var fileDom = XDocument.Load(reader, LoadOptions.SetLineInfo);
            rule.Validate(new DataToolsFileInfo(scriptPath, fileDom));
        }
    }
}
