using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace PFire.Common.Extensions
{
    public static class ValidationResultSubclassExtensions
    {
        public static T AddError<T>(this T result, Exception exception) where T : ValidationResult
        {
            ValidationResultExtensions.AddError(result, exception);

            return result;
        }

        public static T AddError<T>(this T result, string error) where T : ValidationResult
        {
            ValidationResultExtensions.AddError(result, error);

            return result;
        }

        public static T AddError<T>(this T result, string propertyName, string error) where T : ValidationResult
        {
            ValidationResultExtensions.AddError(result, propertyName, error);

            return result;
        }

        public static T1 Merge<T1, T2>(this T1 result, params T2[] otherResults) where T1 : ValidationResult where T2 : ValidationResult
        {
            ValidationResultExtensions.Merge(result, otherResults.AsEnumerable());

            return result;
        }

        public static T1 Merge<T1, T2>(this T1 result, IEnumerable<T2> otherResults) where T1 : ValidationResult where T2 : ValidationResult
        {
            ValidationResultExtensions.Merge(result, otherResults);

            return result;
        }
    }
}
