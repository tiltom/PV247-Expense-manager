namespace ExpenseManager.BusinessLogic.Validators
{
    internal interface IServiceValidation<in T>
    {
        void Validate(T model);
    }
}