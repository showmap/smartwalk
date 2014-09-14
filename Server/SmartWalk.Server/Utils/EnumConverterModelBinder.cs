using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace SmartWalk.Server.Utils
{
    /// <summary>
    /// A custom Binder to properly convert Int values back to Enums.
    /// The code is taken from: http://stackoverflow.com/questions/4582233/asp-net-mvc3-why-does-the-default-support-for-json-model-binding-fail-to-decod
    /// </summary>
    public class EnumConverterModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor,
            IModelBinder propertyBinder)
        {
            var propertyType = propertyDescriptor.PropertyType;
            if (propertyType.IsEnum)
            {
                var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (providerValue != null)
                {
                    var value = providerValue.RawValue;
                    if (value != null)
                    {
                        var valueType = value.GetType();
                        if (!valueType.IsEnum)
                        {
                            try
                            {
                                return Enum.ToObject(propertyType, value);
                            }
                                // ReSharper disable EmptyGeneralCatchClause
                            catch
                            {
                            }
                            // ReSharper restore EmptyGeneralCatchClause
                        }
                    }
                }
            }

            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}