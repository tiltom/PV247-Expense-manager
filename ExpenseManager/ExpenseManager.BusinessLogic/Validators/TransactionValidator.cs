using System;
using ExpenseManager.BusinessLogic.TransactionServices.Models;
using ExpenseManager.Resources.TransactionResources;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class TransactionValidator : AbstractValidator<TransactionServiceModel>
    {
        public TransactionValidator()
        {
            this.RuleFor(transaction => transaction.WalletId)
                .NotEqual(Guid.Empty)
                .WithLocalizedMessage(() => TransactionResource.WalletIdError);
            this.RuleFor(transaction => transaction.Amount)
                .GreaterThan(0)
                .WithLocalizedMessage(() => TransactionResource.AmmountError);
            this.RuleFor(transaction => transaction.Date)
                .NotEqual(DateTime.MinValue)
                .WithLocalizedMessage(() => TransactionResource.DateError);
            this.RuleFor(transaction => transaction.CategoryId)
                .NotEqual(Guid.Empty)
                .WithLocalizedMessage(() => TransactionResource.CategoryError);

            this.RuleFor(transaction => transaction.CurrencyId)
                .NotEqual(Guid.Empty)
                .WithLocalizedMessage(() => TransactionResource.CurrencyError);

            this.When(transaction => transaction.IsRepeatable, () =>
            {
                this.RuleFor(transaction => transaction.LastOccurrence)
                    .NotNull()
                    .WithLocalizedMessage(
                        () =>
                            TransactionResource
                                .LastOccurrenceNullError);
                this.RuleFor(transaction => transaction.LastOccurrence)
                    .NotNull()
                    .GreaterThan(transaction => transaction.Date)
                    .WithLocalizedMessage(() => TransactionResource.LastOccurrenceBeforeDateError);

                this.RuleFor(transaction => transaction.NextRepeat)
                    .NotNull()
                    .GreaterThan(0)
                    .WithLocalizedMessage(() => TransactionResource.NextRepeatError);
            });
        }
    }
}