using FluentValidation.Results;

namespace PFire.Common.Extensions
{
    public static class ValidationFailureExtensions
    {
        public static string GetErrorString(this ValidationFailure failure, bool includeProperty = true)
        {
            return includeProperty && !string.IsNullOrEmpty(failure.PropertyName) ? $"{failure.PropertyName}:{failure.ErrorMessage}" : failure.ErrorMessage;
        }
    }
}
