using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TeamTools.Common.Linting;
using TeamTools.Common.Linting.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Infrastructure;
using TeamTools.SSDT.ProjectValidator.Interfaces;

namespace TeamTools.SSDT.ProjectValidator
{
    public class DataToolsProjectLinter : ILinter
    {
        private readonly LintingConfigLoader loader = new LintingConfigLoader();
        private readonly IRuleClassFinder ruleClassFinder;
        private IRuleCollectionHandler<IDataToolsRule, DataToolsFileInfo> ruleCollection;
        private IReporter reporter;

        public DataToolsProjectLinter() : base()
        {
            this.ruleClassFinder = new RuleClassFinder();
        }

        public DataToolsProjectLinter(IRuleClassFinder classFilder) : base()
        {
            this.ruleClassFinder = classFilder;
        }

        public DataToolsProjectLinter(LintingConfig config, IRuleClassFinder classFilder, IReporter reporter) : base()
        {
            this.ruleClassFinder = classFilder;
            this.Config = config;
            this.reporter = reporter;

            InitRulesCollection();
        }

        public LintingConfig Config { get; protected set; }

        public IDictionary<string, List<RuleInstance<IDataToolsRule>>> Rules => ruleCollection?.Rules;

        public int GetRulesCount()
        {
            return ruleCollection?.RuleCount() ?? 0;
        }

        public void SetReporter(IReporter reporter)
        {
            this.reporter = reporter;
        }

        public IEnumerable<string> GetSupportedFiles()
        {
            return Config.SupportedFileTypes;
        }

        public void Init(string configPath, IReporter reporter, string cultureCode)
        {
            Debug.Assert(configPath != "", "empty config path");
            Debug.Assert(reporter != null, "reporter null");

            if (Config != null)
            {
                throw new InvalidOperationException("Config already loaded");
            }

            SetReporter(reporter);
            LoadConfig(configPath);
            LoadResources(Config, new AssemblyWrapper(), cultureCode);
            InitRulesCollection();
        }

        public virtual void LoadConfig(string configPath)
        {
            Config = loader.LoadConfig(configPath);
        }

        public void PerformAction(ILintingContext context)
        {
            // TODO : or error?
            if (GetRulesCount() == 0)
            {
                return;
            }

            ruleCollection.ApplyRulesTo(context, (rule, contents) => rule.Validate(contents));
        }

        protected void InitRulesCollection()
        {
            ruleCollection = new RuleCollectionHandler(
                reporter,
                new RuleFactory(),
                ruleClassFinder,
                new XmlFileParser(),
                Config);
            ruleCollection.MakeRules();
        }

        private static void LoadResources(LintingConfig cfg, IAssemblyWrapper assemblyWrapper, string cultureCode)
        {
            const string resourceSubfolder = "Resources";
            var loader = new ResourceLoader<LintingConfig>(cfg);
            // This is DLL code and we need path to current DLL
            // but AppDomain.CurrentDomain.BaseDirectory would return EXE path, not DLL
            // TODO : does not look like a real abstraction from infrastructure
            string resFolder = Path.Combine(assemblyWrapper.GetExecutingPath(Assembly.GetExecutingAssembly()), resourceSubfolder);
            string msgFileName = "ViolationMessages.json";
            if (!string.IsNullOrEmpty(cultureCode) && !string.Equals("en-us", cultureCode, StringComparison.InvariantCultureIgnoreCase))
            {
                if (File.Exists(Path.Combine(resFolder, $"ViolationMessages.{cultureCode}.json")))
                {
                    msgFileName = $"ViolationMessages.{cultureCode}.json";
                }
            }

            loader.LoadConfig(Path.Combine(resFolder, msgFileName));
        }
    }
}
