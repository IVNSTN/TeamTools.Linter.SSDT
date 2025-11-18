using NUnit.Framework;
using System.Collections.Generic;
using TeamTools.SSDT.ProjectValidator.Rules.Reports;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    [Category("Linter.SSDT.Reports")]
    [TestOfRule(typeof(IntegratedSecurityInReportRule))]
    public sealed class IntegratedSecurityInReportRuleTests : BaseRuleTest
    {
        [TestCaseSource(nameof(TestCasePresets))]
        public override void TestRule(string scriptPath, int expectedViolationCount)
        {
            CheckRuleViolations(scriptPath, expectedViolationCount);
        }

        private static IEnumerable<object> TestCasePresets()
        {
            return GetTestSources(typeof(IntegratedSecurityInReportRuleTests));
        }
    }
}
