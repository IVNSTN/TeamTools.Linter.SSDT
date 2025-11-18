using NUnit.Framework;
using System.Collections.Generic;
using TeamTools.SSDT.ProjectValidator.Rules.SqlProject;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    [Category("Linter.SSDT.SqlProject")]
    [TestOfRule(typeof(DacpacWarningSuppressedRule))]
    public sealed class DacpacWarningSuppressedRuleTests : BaseRuleTest
    {
        [TestCaseSource(nameof(TestCasePresets))]
        public override void TestRule(string scriptPath, int expectedViolationCount)
        {
            CheckRuleViolations(scriptPath, expectedViolationCount);
        }

        private static IEnumerable<object> TestCasePresets()
        {
            return GetTestSources(typeof(DacpacWarningSuppressedRuleTests));
        }
    }
}
