using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Rules;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    [Category("Linter.SSDT.PluginTests")]
    [TestOf(typeof(DataToolsProjectLinter))]
    public sealed class PluginInstanceTests
    {
        private static string RuleIdSeparator => RuleIdentityAttribute.IdSeparator;

        [Test]
        public void TestLinterAppliesRulesToContextAndInvokesReporter()
        {
            const string ruleId = "FAILINGRULE";

            var reporter = new MockReporter();
            var linter = new MockPlugin(
                [ruleId],
                "XML",
                ".xml",
                reporter,
                new MockRuleClassFinder(typeof(MockFailingRule), "XML"));
            var context = new LintingContext("dummy.xml", new StringReader("<dummy/>"), reporter);
            linter.PerformAction(context);

            Assert.That(reporter.Errors, Has.Count.EqualTo(1));
            Assert.That(reporter.Errors[0], Is.EqualTo("FAILINGRULE: Failed: i always fail"));
        }

        [Test]
        public void TestPluginRunDeliversRuleViolations()
        {
            var reporter = new MockReporter();
            var context = new LintingContext(
                "dummy.rdl",
                new StringReader(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Report MustUnderstand=""df"" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"">
  <EmbeddedImages></EmbeddedImages>
  <DataSource Name=""TESTDS"">
    <ConnectionProperties>
      <ConnectString>Data Source=preproddb;Initial Catalog=reporting_database</ConnectString>
    </ConnectionProperties>
  </DataSource>
</Report>"),
                reporter);

            var plugin = new MockPlugin(
                Array.Empty<string>(),
                "ReportDesign",
                ".rdl",
                reporter,
                new RuleClassFinder());
            plugin.PerformAction(context);
            Assert.That(reporter.Errors, Is.Empty, "no rules - no errors");

            plugin = new MockPlugin(
                new string[]
                {
                    $"RDL0001{RuleIdSeparator}EMBEDDED_IMAGES",
                    $"RDL0003{RuleIdSeparator}CONNECTION_STRING_ILLEGAL",
                },
                "ReportDesign",
                ".rdl",
                reporter,
                new RuleClassFinder());
            plugin.PerformAction(context);

            Assert.That(reporter.Errors, Has.Count.EqualTo(2));
            Assert.That(reporter.Errors.Count(e => e.StartsWith("RDL0001")), Is.EqualTo(1), "rule 1");
            Assert.That(reporter.Errors.Count(e => e.StartsWith("RDL0003")), Is.EqualTo(1), "rule 3");
        }

        // TODO : below code is very similar to TSQL Linter plugin tests
        private class MockPlugin : DataToolsProjectLinter
        {
            private readonly List<string> enabledRules = new List<string>();

            public MockPlugin(string[] rules, string dataType, string fileExt, IReporter reporter, IRuleClassFinder ruleClassFinder) : base(ruleClassFinder)
            {
                enabledRules.AddRange(rules);
                SetReporter(reporter);
                LoadConfig(default);
                Config.SupportedFiles.Add(dataType, new List<string> { fileExt });
                InitRulesCollection();
            }

            public override void LoadConfig(string configPath)
            {
                Config = new LintingConfig();
                foreach (string rule in enabledRules)
                {
                    Config.Rules.Add(rule, rule);
                }
            }
        }

        // TODO : below code is very similar to TSQL Linter plugin tests
        private class MockFailingRule : BaseDataToolsRule
        {
            public MockFailingRule() : base()
            { }

            protected override void DoValidate(DataToolsFileInfo fileInfo)
            {
                throw new Exception("i always fail");
            }
        }

        private class MockRuleClassFinder : IRuleClassFinder
        {
            private readonly Type ruleClass;
            private readonly string dataTypeName;

            public MockRuleClassFinder(Type ruleClass, string dataTypeName)
            {
                this.ruleClass = ruleClass;
                this.dataTypeName = dataTypeName;
            }

            public IEnumerable<RuleClassInfoDto> GetAvailableRuleClasses(IDictionary<string, string> enabledRuleIds)
            {
                if (ruleClass is null)
                {
                    yield break;
                }

                // TODO : or at least compare rule.ID with existing enabledRuleIds?
                yield return new RuleClassInfoDto
                {
                    RuleClassType = this.ruleClass,
                    SupportedDataTypes = new[] { dataTypeName },
                    RuleFullName = enabledRuleIds.Keys.First(),
                    RuleId = enabledRuleIds.Keys.FirstOrDefault(),
                };
            }
        }

        private class MockReporter : IReporter
        {
            public IList<string> Errors { get; } = new List<string>();

            public IList<RuleViolation> Violations { get; } = new List<RuleViolation>();

            public void Report(string error)
            {
                Errors.Add(error);
            }

            public void ReportFailure(string error)
            {
                Errors.Add(error);
            }

            public void ReportViolation(RuleViolation violation)
            {
                Errors.Add($"{violation.RuleId}: {violation.Text}");
                Violations.Add(violation);
            }
        }
    }
}
