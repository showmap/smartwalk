using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartWalk.Labs.Api.DataContracts;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Labs.Api
{
    // ReSharper disable LocalizableElement
    public class MainClass
    {
#if LOCAL
        private const string SmartWalkUrl = "http://localhost/api";
#else
        private const string SmartWalkUrl = "http://dev.smartwalk.me/api";
#endif

        public static void Main(string[] args)
        {
            Console.WriteLine("SmartWalk Protocol JSON objects demo");
            Console.WriteLine("\nHome View");

            var homeViewRequest = RequestFactory.CreateHomeViewRequest(37.757671, -122.408406);
            homeViewRequest.ClientVersion = "2.0.0";
            var json = JsonConvert.SerializeObject(
                homeViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            var eventId = 0;
            var hostId = 0;

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);

                    Console.WriteLine("Response hash code: " + response.GetHashCode());

                    // extract EventMetadata instances
                    var eventMetadatas = response
                        .Selects[0].Records
                        .Cast<JObject>()
                        .Select(r => r.ToObject<EventMetadata>()).ToArray();

                    eventId = eventMetadatas.Select(em => em.Id).FirstOrDefault();
                    hostId = eventMetadatas
                        .Select(em => em.Host.Select(h => h.Id).FirstOrDefault())
                        .FirstOrDefault();

                    Console.WriteLine("EventMetadatas hash code: " + eventMetadatas.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("\nEvent View");
            var eventViewRequest = RequestFactory.CreateEventViewRequest(eventId);
            eventViewRequest.ClientVersion = "2.0.0";
            json = JsonConvert.SerializeObject(
                eventViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                    Console.WriteLine("Response hash code: " + response.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("\nVenues View");
            var venueViewRequest = RequestFactory.CreateVenuesViewRequest(eventId);
            venueViewRequest.ClientVersion = "2.0.0";
            json = JsonConvert.SerializeObject(
                venueViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                    Console.WriteLine("Response hash code: " + response.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("\nHost View");
            var hostViewRequest = RequestFactory.CreateHostViewRequest(hostId);
            hostViewRequest.ClientVersion = "2.0.0";
            json = JsonConvert.SerializeObject(
                hostViewRequest,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                    Console.WriteLine("Response hash code: " + response.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.ReadLine();
        }
    }
    // ReSharper restore LocalizableElement
}