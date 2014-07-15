using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SmartWalk.Server.Extensions
{
    public class JsonNetResult : JsonResult
    {
        public static readonly JsonSerializerSettings Settings;

        static JsonNetResult()
        {
            Settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Error,
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            var response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            if (ContentEncoding != null) response.ContentEncoding = ContentEncoding;
            if (Data == null) return;

            var scriptSerializer = JsonSerializer.Create(Settings);

            using (var sw = new StringWriter())
            {
                scriptSerializer.Serialize(sw, Data);
                response.Write(sw.ToString());
            }
        }
    }
}