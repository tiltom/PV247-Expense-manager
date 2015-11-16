using System;
using System.Globalization;
using System.Web.Mvc;

namespace ExpenseManager.Web.Binders
{
    public class DateModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var attemptedValue =
                bindingContext.ValueProvider.GetValue(modelName).AttemptedValue;

            if (bindingContext.ModelMetadata.IsNullableValueType
                && string.IsNullOrWhiteSpace(attemptedValue))
            {
                return null;
            }

            DateTime value;
            var valid = DateTime.TryParseExact(attemptedValue, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

            if (!valid)
            {
                var result = base.BindModel(controllerContext, bindingContext);
                bindingContext.ModelState.AddModelError(modelName, "Date was not in format dd.MM.yyyy");
                return result;
            }

            return value;
        }
    }
}