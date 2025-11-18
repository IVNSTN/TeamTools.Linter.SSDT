using NUnit.Framework;
using System;
using System.IO;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    // TODO : very similar to TSQL.LinterTests
    public sealed class RuleTestCasePreset : TestCaseData
    {
        public RuleTestCasePreset(string testSourceFile, int expectedViolations) : base(testSourceFile, expectedViolations)
        {
            if (string.IsNullOrWhiteSpace(testSourceFile))
            {
                throw new ArgumentNullException(nameof(testSourceFile));
            }

            if (expectedViolations < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedViolations), "cannot be negative");
            }

            SetName(string.Concat("{c}.", Path.GetFileNameWithoutExtension(testSourceFile)));

            // FIXME: too much magic
            Properties.Add("_CodeFilePath", testSourceFile.Replace(@"\bin\debug\net8.0\", @"\", StringComparison.OrdinalIgnoreCase));
            Properties.Add("_LineNumber", 1);
        }
    }
}
