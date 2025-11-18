using NUnit.Framework;
using System;
using System.IO;
using TeamTools.Common.Linting;
using TeamTools.Common.Linting.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Infrastructure;

namespace TeamTools.SSDT.ProjectValidatorTests.PluginTests
{
    [Category("Linter.SSDT.PluginTests")]
    public sealed class LintingConfigLoaderTests
    {
        private static string RuleIdSeparator => RuleIdentityAttribute.IdSeparator;

        [Test]
        public void ConfigLoadingReturnsNullIfFileNotFound()
        {
            var loader = new LintingConfigLoader();
            try
            {
                Assert.That(loader.LoadConfig("dummy path"), Is.Null);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void ConfigLoaderReadsAllBlocksFromDefaultConfig()
        {
            var loader = new LintingConfigLoader();
            var configPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "DefaultConfig.json");

            Assert.That(File.Exists(configPath), Is.True);
            var config = loader.LoadConfig(configPath);

            Assert.That(config, Is.Not.Null);
            Assert.That(config.SupportedFiles, Is.Not.Empty, "SupportedFiles missing");
            Assert.That(config.Rules, Is.Not.Empty, "Rules missing");
        }

        [Test]
        public void TestResourceFileContainsMessages()
        {
            string testedRuleId = $"RDL0001{RuleIdSeparator}EMBEDDED_IMAGES";
            var config = new LintingConfig();
            config.Rules.Add(testedRuleId, "dummy"); // messages loader does not add new rules
            var resourceLoader = new ResourceLoader<LintingConfig>(config);
            string resPath = GetResourceFilePath("ViolationMessages.json");
            using var res = new StreamReader(resPath);
            resourceLoader.LoadConfig(res);

            Assert.That(config.Rules[testedRuleId], Is.Not.EqualTo("dummy"));
        }

        private string GetResourceFilePath(string fileName)
        {
            return Path.Join(TestContext.CurrentContext.TestDirectory, "Resources", fileName);
        }
    }
}
