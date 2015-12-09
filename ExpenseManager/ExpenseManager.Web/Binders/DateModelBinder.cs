using System;
using System.Globalization;
using System.Web.Mvc;
using ExpenseManager.Web.Controllers;

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
            var valid = DateTime.TryParseExact(attemptedValue, AbstractController.DateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

            if (!valid)
            {
                return base.BindModel(controllerContext, bindingContext);
            }

            return value;
        }
    }
}