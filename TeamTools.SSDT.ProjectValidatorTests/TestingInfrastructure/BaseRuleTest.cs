using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TeamTools.SSDT.ProjectValidator.Rules;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    // TODO : very similar to TSQL.LinterTests
    public abstract partial class BaseRuleTest
    {
        private const string UnitTestsSubfolder = "UnitTests";
        private const string TestSourcesSubfolder = "TestSources";
        private const string RootTestCategory = "Linter.SSDT.";
        private const string ViolationCounterMatchGroup = "violations";
        private static readonly Regex ExpectedViolationsPattern = MyRegex();
        private MockLinter linterInstance;

        protected BaseRuleTest() : base()
        {
            var ruleClassAttr = this.GetType().GetCustomAttributes(typeof(TestOfRuleAttribute), false).FirstOrDefault();
            if (ruleClassAttr != null)
            {
                RuleClass = (ruleClassAttr as TestOfRuleAttribute).RuleClass;
            }
        }

        protected Type RuleClass { get; }

        [SetUp]
        public void SetUp()
        {
            linterInstance = new MockLinter();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", "NUnit1028:The non-test method is public", Justification = "This is an abstraction for descendants")]
        public abstract void TestRule(string scriptPath, int expectedViolationCount);

        protected static IEnumerable<object> GetTestSources(Type testClass)
        {
            string ruleClassName = default;
            string ruleCategoryName = default;

            var ruleClassAttr = testClass.GetCustomAttributes(typeof(TestOfRuleAttribute), false).FirstOrDefault();
            if (ruleClassAttr != null)
            {
                ruleClassName = (ruleClassAttr as TestOfRuleAttribute).RuleClass.Name;
            }

            if (string.IsNullOrEmpty(ruleClassName))
            {
                yield break;
            }

            var categoryAttr = testClass.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault();
            if (categoryAttr != null)
            {
                ruleCategoryName = (categoryAttr as CategoryAttribute).Name.Replace(RootTestCategory, "");
            }

            if (string.IsNullOrEmpty(ruleCategoryName))
            {
                yield break;
            }

            string scriptPathFolder = Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                UnitTestsSubfolder,
                ruleCategoryName,
                ruleClassName,
                TestSourcesSubfolder));

            foreach (var file in Directory.EnumerateFiles(scriptPathFolder))
            {
                var match = ExpectedViolationsPattern.Match(Path.GetFileNameWithoutExtension(file));
                if (match is null
                || !int.TryParse(match.Groups[ViolationCounterMatchGroup].Value, out int violationCount))
                {
                    Assert.Fail(string.Format("File name does not match expected pattern: {0}", Path.GetFileName(file)));
                    continue;
                }

                yield return new RuleTestCasePreset(
                    testSourceFile: file,
                    expectedViolations: violationCount);
            }
        }

        protected void CheckRuleViolations(string scriptPath, int expectedViolationCount)
        {
            if (RuleClass is null)
            {
                throw new InvalidOperationException($"{nameof(RuleClass)} must be defined by {nameof(TestOfRuleAttribute)} class attribute");
            }

            if (string.IsNullOrEmpty(scriptPath))
            {
                throw new ArgumentNullException(nameof(scriptPath));
            }

            if (expectedViolationCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedViolationCount), "cannot be negative");
            }

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("Not found test case source file: " + scriptPath);
            }

            int errCnt = 0;
            var rule = (BaseDataToolsRule)Activator.CreateInstance(RuleClass);
            rule.ViolationCallback += (obj, dto) => errCnt++;

            try
            {
                linterInstance.Lint(scriptPath, rule);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.That(errCnt, Is.EqualTo(expectedViolationCount), "Expected violation count mismatched");
        }

        [GeneratedRegex(@".*raise_(?<violations>[\d]+)_violations", RegexOptions.IgnoreCase | RegexOptions.Compiled, "ru-RU")]
        private static partial Regex MyRegex();
    }
}
