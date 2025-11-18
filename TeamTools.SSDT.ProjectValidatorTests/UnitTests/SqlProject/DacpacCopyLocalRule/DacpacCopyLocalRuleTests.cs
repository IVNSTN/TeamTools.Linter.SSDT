using NUnit.Framework;
using System.Collections.Generic;
using TeamTools.SSDT.ProjectValidator.Rules.SqlProject;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    [Category("Linter.SSDT.SqlProject")]
    [TestOfRule(typeof(DacpacCopyLocalRule))]
    public sealed class DacpacCopyLocalRuleTests : BaseRuleTest
    {
        [TestCaseSource(nameof(TestCasePresets))]
        public override void TestRule(string scriptPath, int expectedViolationCount)
        {
            CheckRuleViolations(scriptPath, expectedViolationCount);
        }

        private static IEnumerable<object> TestCasePresets()
        {
            return GetTestSources(typeof(DacpacCopyLocalRuleTests));
        }
    }
}
