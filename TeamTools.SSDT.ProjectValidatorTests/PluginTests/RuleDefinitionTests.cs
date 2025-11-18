using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Rules;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    // TODO : very similar to TSQL.LinterTests
    [Category("Linter.SSDT.PluginTests")]
    public sealed class RuleDefinitionTests
    {
        [Test]
        public void TestAllRulesHaveIdentity()
        {
            var rules = (
                from assm in AppDomain.CurrentDomain.GetAssemblies()
                from assmType in assm.GetTypes()
                where assmType.IsSubclassOf(typeof(BaseDataToolsRule))
                    && !assmType.IsAbstract
                    && !assmType.Name.StartsWith("Mock")
                select assmType)
                .ToList();

            Assert.That(rules, Is.Not.Empty, "no rule classes found");

            string rulesWithNoId = string.Join(
                Environment.NewLine,
                rules
                .Where(r => r.GetCustomAttributes(typeof(RuleIdentityAttribute), true)?.Length == 0)
                .Select(r => r.Name)
                .ToList());

            Assert.That(rulesWithNoId, Is.EqualTo(""), rulesWithNoId);
        }

        [Test]
        public void TestAllRulesHaveDataTypeLink()
        {
            var rules = (
                from assm in AppDomain.CurrentDomain.GetAssemblies()
                from assmType in assm.GetTypes()
                where assmType.IsSubclassOf(typeof(BaseDataToolsRule))
                    && !assmType.IsAbstract
                    && !assmType.Name.StartsWith("Mock")
                select assmType)
                .ToList();

            Assert.That(rules, Is.Not.Empty, "no rule classes found");

            string rulesWithNoDataType = string.Join(
                Environment.NewLine,
                rules
                .Where(r => r.GetCustomAttributes(typeof(DataTypeAttribute), true)?.Length == 0)
                .Select(r => r.Name)
                .ToList());

            Assert.That(rulesWithNoDataType, Is.EqualTo(""), rulesWithNoDataType);
        }

        [Test]
        public void TestAllRuleIdsMentionedInDefaultConfig()
        {
            var ruleIds = (
                from assm in AppDomain.CurrentDomain.GetAssemblies()
                from assmType in assm.GetTypes()
                where assmType.IsSubclassOf(typeof(BaseDataToolsRule))
                    && !assmType.IsAbstract
                    && !assmType.Name.StartsWith("Mock")
                    && assmType.GetCustomAttributes(typeof(RuleIdentityAttribute), true)?.Length > 0
                select (assmType.GetCustomAttributes(typeof(RuleIdentityAttribute), true).FirstOrDefault() as RuleIdentityAttribute).FullName)
                .ToList();

            Assert.That(ruleIds, Is.Not.Empty, "no rule classes found");

            var loader = new MockConfigIdentityLoader();
            string defaultConfPath = Path.Join(TestContext.CurrentContext.TestDirectory, "DefaultConfig.json");
            Assert.That(File.Exists(defaultConfPath), Is.True, "Config not found: " + defaultConfPath);
            var config = loader.LoadConfig(defaultConfPath);

            Assert.That(config, Is.Not.Null, "config null");
            Assert.That(config.Rules, Is.Not.Empty, "rules from conf");

            var rulesNotInConfig = ruleIds.Except(config.Rules.Keys).ToList();

            Assert.That(rulesNotInConfig, Is.Empty, string.Join(",", rulesNotInConfig));
        }

        // TODO : same as in TSQL.LinterTests
        private class MockConfigIdentityLoader : LintingConfigLoader
        {
            protected override void FillConfig(LintingConfig config, JToken json)
            {
                var rulesConf = json.SelectTokens("..rules").ToList();

                foreach (var configValue in rulesConf.Children())
                {
                    var prop = (JProperty)configValue;
                    // loading all, even disabled rules
                    config.Rules.Add(prop.Name, prop.Name);
                }
            }
        }
    }
}
