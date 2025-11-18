using NUnit.Framework;
using System.Collections.Generic;
using TeamTools.SSDT.ProjectValidator.Rules.ReportDatasource;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    [Category("Linter.SSDT.ReportDatasource")]
    [TestOfRule(typeof(ServerNameFullyQualifiedRule))]
    public sealed class ServerNameFullyQualifiedRuleTests : BaseRuleTest
    {
        [TestCaseSource(nameof(TestCasePresets))]
        public override void TestRule(string scriptPath, int expectedViolationCount)
        {
            CheckRuleViolations(scriptPath, expectedViolationCount);
        }

        private static IEnumerable<object> TestCasePresets()
        {
            return GetTestSources(typeof(ServerNameFullyQualifiedRuleTests));
        }
    }
}
