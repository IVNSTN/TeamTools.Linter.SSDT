using NUnit.Framework;
using System;
using TeamTools.SSDT.ProjectValidator.Rules;

namespace TeamTools.SSDT.ProjectValidatorTests
{
    // TODO : very similar to TSQL.LinterTests
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TestOfRuleAttribute : TestOfAttribute
    {
        public TestOfRuleAttribute(Type ruleClass) : base(ruleClass)
        {
            RuleClass = ruleClass ?? throw new ArgumentNullException(nameof(ruleClass));

            if (ruleClass.IsAssignableFrom(typeof(BaseDataToolsRule)))
            {
                throw new ArgumentOutOfRangeException(nameof(ruleClass), "must be subclass of " + nameof(BaseDataToolsRule));
            }
        }

        public Type RuleClass { get; }
    }
}
