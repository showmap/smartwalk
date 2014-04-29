using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ModernHttpClient;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Services
{
    public class HttpService : IHttpService
    {
        private const string JsonHeader = "application/json";

        private readonly IConfiguration _configuration;

        public HttpService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TResponse> Get<TResponse>(object request)
            where TResponse : class
        {
            if (request == null) throw new ArgumentNullException("request");

            var serialized = JsonConvert.SerializeObject(request);
            var content = new StringContent(serialized);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(JsonHeader);

            var httpClient = CreateHttpClient();

            var response = await httpClient.PostAsync(_configuration.Api, content);
            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TResponse>(resultString);
            return result;
        }

        public static HttpClient CreateHttpClient()
        {
            HttpClient result;

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                result = new HttpClient(new NSUrlSessionHandler());
            }
            else
            {
                result = new HttpClient(new HttpClientHandler());
            }

            return result;
        }
    }
}