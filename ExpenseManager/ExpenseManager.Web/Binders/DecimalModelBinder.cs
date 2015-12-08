using System;
using System.Globalization;
using System.Web.Mvc;

namespace ExpenseManager.Web.Binders
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            object result = null;

            var modelName = bindingContext.ModelName;
            var attemptedValue =
                bindingContext.ValueProvider.GetValue(modelName).AttemptedValue;

            attemptedValue = UniversalSeparator(attemptedValue);

            try
            {
                if (bindingContext.ModelMetadata.IsNullableValueType
                    && string.IsNullOrWhiteSpace(attemptedValue))
                {
                    return null;
                }

                result = decimal.Parse(attemptedValue, NumberStyles.Any);
            }
            catch (FormatException e)
            {
                result = base.BindModel(controllerContext, bindingContext);
                bindingContext.ModelState.AddModelError(modelName, e);
            }

            catch (OverflowException e)
            {
                result = base.BindModel(controllerContext, bindingContext);
                bindingContext.ModelState.AddModelError(modelName, e);
            }

            return result;
        }

        private static string UniversalSeparator(string attemptedValue)
        {
            // Depending on CultureInfo, the NumberDecimalSeparator can be "," or "."
            // Both "." and "," should be accepted, but aren't.
            var wantedSeperator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var alternateSeperator = (wantedSeperator == "," ? "." : ",");

            if (attemptedValue.IndexOf(wantedSeperator, StringComparison.Ordinal) == -1
                && attemptedValue.IndexOf(alternateSeperator, StringComparison.Ordinal) != -1)
            {
                attemptedValue =
                    attemptedValue.Replace(alternateSeperator, wantedSeperator);
            }
            return attemptedValue;
        }
    }
}