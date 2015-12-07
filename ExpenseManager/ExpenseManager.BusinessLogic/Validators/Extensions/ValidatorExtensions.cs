using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators.Extensions
{
    internal static class ValidatorExtensions
    {
        /// <summary>
        ///     Performs validation and then throws an custom exception if validation fails.
        /// </summary>
        public static void ValidateAndThrowCustomException<T>(this IValidator<T> validator, T instance)
        {
            var result = validator.Validate(instance);

            if (!result.IsValid)
            {
                throw new ServiceValidationException(result.Errors);
            }
        }
    }
}