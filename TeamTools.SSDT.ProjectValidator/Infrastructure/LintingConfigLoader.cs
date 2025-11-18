using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TeamTools.Common.Linting;

namespace TeamTools.SSDT.ProjectValidator.Infrastructure
{
    // TODO : very similar to StructuredFiles.LintingConfigLoader
    internal class LintingConfigLoader : BaseJsonConfigLoader<LintingConfig>
    {
        private const string TypeAssociationsNode = "FileTypes";
        private const string ViolationMessagesNode = "messages";
        private const string RulesNode = "rules";

        protected override void FillConfig(LintingConfig config, JToken src)
        {
            ActivateSupportedFileTypes(config, src);
            ActivateRules(config, src);
            TranslateRules(config, src);
        }

        protected override LintingConfig MakeConfig()
        {
            return new LintingConfig();
        }

        protected void ActivateSupportedFileTypes(LintingConfig config, JToken json)
        {
            Debug.WriteLine("ActivateSupportedFileTypes");
            Debug.Assert(config != null, "config not set");
            Debug.Assert(json != null, "json not set");
            var typesConf = json.SelectTokens(".." + TypeAssociationsNode)
                .Children()
                .OfType<JProperty>();

            foreach (var prop in typesConf)
            {
                string dataType = prop.Name;
                if (prop.Value is JArray a)
                {
                    config.SupportedFiles.Add(dataType, new List<string>(a.Values<string>()));
                }
            }
        }

        protected void ActivateRules(LintingConfig config, JToken json)
        {
            Debug.WriteLine("ActivateRules");
            Debug.Assert(config != null, "config not set");
            Debug.Assert(json != null, "json not set");
            var typesConf = json.SelectTokens(".." + RulesNode)
                .Children()
                .OfType<JProperty>();

            foreach (var prop in typesConf)
            {
                var severity = SeverityConverter.ConvertFromString(prop.Value.ToString());
                string ruleId = prop.Name;

                if (severity != Severity.None)
                {
                    config.Rules.Add(ruleId, ruleId);
                    config.RuleSeverity.Add(ruleId, severity);
                }
            }
        }

        // TODO : copy-pasted from TSQL linter config loader
        protected void TranslateRules(LintingConfig config, JToken json)
        {
            Debug.WriteLine("TranslateRules");
            Debug.Assert(config != null, "config not set");
            Debug.Assert(json != null, "json not set");

            var rulesTranslations = json
                .SelectTokens(".." + ViolationMessagesNode)
                .Children()
                .OfType<JProperty>();

            foreach (var prop in rulesTranslations)
            {
                if (config.Rules.ContainsKey(prop.Name))
                {
                    config.Rules[prop.Name] = prop.Value.ToString();
                }
            }
        }
    }
}
