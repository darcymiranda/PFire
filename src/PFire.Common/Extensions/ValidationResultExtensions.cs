using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace PFire.Common.Extensions
{
    public static class ValidationResultExtensions
    {
        public static string GetErrorString(this ValidationResult result, bool includeProperty = true, string delimiter = null)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.IsValid)
            {
                return null;
            }

            delimiter ??= Environment.NewLine;

            return result.Errors.Select(x => x.GetErrorString(includeProperty)).Join(delimiter);
        }

        public static ValidationResult AddError(this ValidationResult result, Exception exception)
        {
            return result.AddError(exception.GetFullMessage());
        }

        public static ValidationResult AddError(this ValidationResult result, string errorMessage)
        {
            return result.AddError(string.Empty, errorMessage);
        }

        public static ValidationResult AddError(this ValidationResult result, string propertyName, string errorMessage)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            result.Errors.Add(new ValidationFailure(propertyName, errorMessage));

            return result;
        }

        public static ValidationResult Merge(this ValidationResult result, IEnumerable<ValidationResult> otherResults)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (otherResults == null)
            {
                return result;
            }

            foreach (var otherResult in otherResults)
            {
                if (otherResult.IsValid)
                {
                    continue;
                }

                foreach (var error in otherResult.Errors)
                {
                    result.Errors.Add(error);
                }
            }

            return result;
        }

        public static ValidationResult Merge(this ValidationResult result, params ValidationResult[] otherResults)
        {
            return result.Merge(otherResults.AsEnumerable());
        }
    }
}
