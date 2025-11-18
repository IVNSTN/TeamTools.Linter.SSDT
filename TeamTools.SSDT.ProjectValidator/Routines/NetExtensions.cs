using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TeamTools.SSDT.ProjectValidator.Routines
{
    [ExcludeFromCodeCoverage]
    public static class NetExtensions
    {
        public static IEnumerable<T> CombineEnumerables<T>(params IEnumerable<T>[] items)
        {
            return items.SelectMany(x => x);
        }
    }
}
