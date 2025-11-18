using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Interfaces;

namespace TeamTools.SSDT.ProjectValidator.Infrastructure
{
    internal sealed class RuleCollectionHandler : BaseRuleCollectionHandler<IDataToolsRule, DataToolsFileInfo>
    {
        public RuleCollectionHandler(IReporter reporter, IRuleFactory<IDataToolsRule> rulesFactory, IRuleClassFinder ruleClassFinder, IFileParser<DataToolsFileInfo> parser, BaseLintingConfig config) : base(reporter, rulesFactory, ruleClassFinder, parser, config)
        {
        }
    }
}
