using System;
using System.Diagnostics;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Interfaces;

namespace TeamTools.SSDT.ProjectValidator.Infrastructure
{
    internal class RuleFactory : IRuleFactory<IDataToolsRule>
    {
        public IDataToolsRule MakeRule(Type ruleClass, ViolationCallbackEvent callback)
        {
            Debug.Assert(typeof(IDataToolsRule).IsAssignableFrom(ruleClass), "Wrong rule class type");

            var rule = Activator.CreateInstance(ruleClass) as IDataToolsRule;
            rule.Subscribe(callback);

            return rule;
        }
    }
}
