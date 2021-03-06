﻿using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseManager.BusinessLogic.ServicesConstants;
using ExpenseManager.Resources;
using FluentValidation.Results;

namespace ExpenseManager.BusinessLogic
{
    public class ServiceValidationException : Exception
    {
        public ServiceValidationException()
        {
        }

        public ServiceValidationException(IEnumerable<ValidationFailure> errors)
            : base(BuildErrorMesage(errors))
        {
            var errorsWithMessages = errors
                .GroupBy(error => error.PropertyName)
                .Select(error => new
                {
                    PropertyName = error.Key,
                    ErrorMessage = string.Join(Environment.NewLine,
                        error.Select(failure => failure.ErrorMessage))
                });

            foreach (var errorsWithMessage in errorsWithMessages)
            {
                Errors[errorsWithMessage.PropertyName] = errorsWithMessage.ErrorMessage;
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

        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        private static string BuildErrorMesage(IEnumerable<ValidationFailure> errors)
        {
            var arr = errors.Select(error => ValidationConstant.ErrorLineDelimeter + error.ErrorMessage).ToArray();
            return string.Format(SharedResource.ValidationFailed, string.Join("", arr));
        }
    }
}