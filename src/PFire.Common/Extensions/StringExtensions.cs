using System;

namespace PFire.Common.Extensions
{
    public static class StringExtensions
    {
        private static T? ToNullableEnum<T>(this string input) where T : struct, Enum
        {
            if (Enum.TryParse(input, true, out T output))
            {
                return output;
            }

            return null;
        }

        public static T ToEnum<T>(this string input) where T : struct, Enum
        {
            return input.ToNullableEnum<T>() ?? throw new ArgumentException($"{input} is not a value for enum {typeof(T).FullName}");
        }
    }
}
