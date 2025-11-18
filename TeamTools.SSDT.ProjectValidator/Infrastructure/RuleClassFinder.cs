using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamTools.Common.Linting;
using TeamTools.SSDT.ProjectValidator.Interfaces;

namespace TeamTools.SSDT.ProjectValidator.Infrastructure
{
    // TODO : looks very similar to TSQL linter RuleClassFinder
    internal class RuleClassFinder : IRuleClassFinder
    {
        public IEnumerable<RuleClassInfoDto> GetAvailableRuleClasses(IDictionary<string, string> enabledRuleIds)
        {
            return
                from t in Assembly.GetExecutingAssembly().GetTypes()
                where !t.IsAbstract && typeof(IDataToolsRule).IsAssignableFrom(t)
                let id = t.GetAttributeValue((RuleIdentityAttribute attr) => attr)
                let dt = t.GetCustomAttributes(typeof(DataTypeAttribute), true)
                    .Select(attr => ((DataTypeAttribute)attr).DataTypeName)
                    .ToArray()
                where id != null && enabledRuleIds.ContainsKey(id.FullName)
                    && dt?.Length > 0
                select new RuleClassInfoDto
                {
                    RuleClassType = t,
                    SupportedDataTypes = dt,
                    RuleFullName = id.FullName,
                    RuleId = id.Id,
                    RuleMnemo = id.Mnemo,
                };
        }
    }
}
