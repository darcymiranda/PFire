using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace PFire.Data.Validators
{
    /// <summary>
    ///     Wrapper around FluentValidation.AbstractValidator to run DataAnnotations validations in addition to regular
    ///     FluentValidations
    /// </summary>
    internal class DataAnnotationsValidator<T> : AbstractValidator<T>
    {
        public override ValidationResult Validate(ValidationContext<T> context)
        {
            var validationResult = base.Validate(context);

            var validationResults = GetDataAnnotationsValidationFailures(context);
            foreach (var result in validationResults)
            {
                validationResult.Errors.Add(result);
            }

            return validationResult;
        }

        private IEnumerable<ValidationFailure> GetDataAnnotationsValidationFailures(ValidationContext<T> context)
        {
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            var validationContext = new ValidationContext(context.InstanceToValidate, null, null);
            Validator.TryValidateObject(context.InstanceToValidate, validationContext, validationResults);

            return validationResults.SelectMany(x => x.MemberNames, (x, y) => new ValidationFailure(y, x.ErrorMessage));
        }
    }
}
