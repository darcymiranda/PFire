using System.Collections.Generic;

namespace PFire.Common.Extensions
{
    public static class EnumerableOfStringExtensions
    {
        public static string Join(this IEnumerable<string> source, string delimiter = ",")
        {
            return source == null ? null : string.Join(delimiter, source);
        }
    }
}
