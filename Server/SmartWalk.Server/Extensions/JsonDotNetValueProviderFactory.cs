using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SmartWalk.Server.Extensions
{
    public sealed class JsonDotNetValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null) throw new ArgumentNullException("controllerContext");

            if (!controllerContext.HttpContext.Request.ContentType.StartsWith(
                "application/json", 
                StringComparison.OrdinalIgnoreCase)) return null;

            using (var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream))
            {
                var bodyText = reader.ReadToEnd();
                var objects = 
                    !String.IsNullOrEmpty(bodyText)
                        ? JsonConvert.DeserializeObject<ExpandoObject>(bodyText, new ExpandoObjectConverter())
                        : null;
                return objects != null
                    ? new DictionaryValueProvider<object>(objects, CultureInfo.CurrentCulture)
                    : null;
            }
        }
    }
}