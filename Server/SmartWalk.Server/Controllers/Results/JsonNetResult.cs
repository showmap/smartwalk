using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SmartWalk.Server.Controllers.Results
{
    public class JsonNetResult : JsonResult
    {
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                response.Write(JsonConvert.SerializeObject(Data, JsonSerializerSettings));
            }
        }
    }
}