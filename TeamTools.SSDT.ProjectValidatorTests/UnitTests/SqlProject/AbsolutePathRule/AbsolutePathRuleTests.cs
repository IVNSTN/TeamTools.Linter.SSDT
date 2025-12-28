using NUnit.Framework;
using System.IO;
using System.Collections.Generic;
using TeamTools.SSDT.ProjectValidator.Rules.SqlProject;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    [Category("Linter.SSDT.SqlProject")]
    [TestOfRule(typeof(AbsolutePathRule))]
    public sealed class AbsolutePathRuleTests : BaseRuleTest
    {
        [TestCaseSource(nameof(TestCasePresets))]
        public override void TestRule(string scriptPath, int expectedViolationCount)
        {
#if Linux
            CheckRuleViolations(Path.Combine(Path.GetDirectoryName(scriptPath), "Unix", Path.GetFileName(scriptPath)), expectedViolationCount);
#else
            CheckRuleViolations(scriptPath, expectedViolationCount);
#endif
        }

        private static IEnumerable<object> TestCasePresets()
        {
            return GetTestSources(typeof(AbsolutePathRuleTests));
        }
    }
}
