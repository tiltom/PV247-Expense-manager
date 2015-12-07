using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace ExpenseManager.BusinessLogic
{
    public class ServiceValidationException : Exception
    {
        public Dictionary<string, string> _errors = new Dictionary<string, string>();

        public ServiceValidationException()
        {
        }

        public ServiceValidationException(IEnumerable<ValidationFailure> errors)
            : base(BuildErrorMesage(errors))
        {
            foreach (var error in errors)
            {
                if (this._errors.ContainsKey(error.PropertyName))
                {
                    this._errors[error.PropertyName] =
                        this._errors[error.PropertyName] +
                        Environment.NewLine +
                        error.ErrorMessage;
                }
                else
                {
                    this._errors.Add(error.PropertyName, error.ErrorMessage);
                }
            }
        }

        public ServiceValidationException(string message)
            : base(message)
        {
        }

        public ServiceValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public Dictionary<string, string> Errors
        {
            get { return this._errors; }
        }

        private static string BuildErrorMesage(IEnumerable<ValidationFailure> errors)
        {
            var arr = errors.Select(x => "\r\n -- " + x.ErrorMessage).ToArray();
            return "Validation failed: " + string.Join("", arr);
        }
    }
}