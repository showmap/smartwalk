using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartWalk.Labs.Api.DataContracts;
using SmartWalk.Labs.Api.Model;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Labs.Api
{
	public class ApiService
	{
		private readonly string _url;

		public ApiService(string url)
		{
			_url = url;
		}

		public Tuple<OrgEvent[], string> GetOrgEvents()
		{
			var request = NewRequestFactory.CreateOrgEventsRequest();
			var response = GetResponse(request);
			var result = default(Tuple<OrgEvent[], string>);

			if (response != null)
			{
				var eventMetadatas = response.Item1
					.GetRecords<EventMetadata>(0)
					.ToArray();

				var hosts = response.Item1
					.GetRecords<Entity>(1)
					.ToArray();

				var orgEvents = eventMetadatas
					.Select(em => 
						new OrgEvent(em, hosts.First(h => h.Id == em.Host.Id())))
					.ToArray();

				result = new Tuple<OrgEvent[], string>(orgEvents, response.Item2);
			}

			return result;
		}

		public Tuple<OrgEvent, string> GetOrgEvent(int id)
		{
			var request = NewRequestFactory.CreateOrgEventRequest(id);
			var response = GetResponse(request);
			var result = default(Tuple<OrgEvent, string>);

			if (response != null)
			{
				var eventMetadata = response.Item1
					.GetRecords<EventMetadata>(0)
					.FirstOrDefault();

				var host = response.Item1
					.GetRecords<Entity>(1)
					.FirstOrDefault();

				var shows = response.Item1
					.GetRecords<Show>(2)
					.ToArray();

				var venueDetails = response.Item1
					.GetRecords<EventVenueDetail>(3)
					.ToArray();

				var venues = response.Item1
					.GetRecords<Entity>(4)
					.Select(e =>
						{
							var venue = CreateVenue(e, venueDetails, shows);
							return venue;
						})
					.ToArray();

				var orgEvent = new OrgEvent(eventMetadata, host, venues);

				result = new Tuple<OrgEvent, string>(orgEvent, response.Item2);
			}

			return result;
		}

		public Tuple<Venue[], string> GetOrgEventVenues(int id)
		{
			var request = NewRequestFactory.CreateOrgEventVenuesRequest(id);
			var response = GetResponse(request);
			var result = default(Tuple<Venue[], string>);

			if (response != null)
			{
				var eventMetadata = response.Item1
					.GetRecords<EventMetadata>(0)
					.FirstOrDefault();

				var shows = response.Item1
					.GetRecords<Show>(1)
					.ToArray();

				var venueDetails = response.Item1
					.GetRecords<EventVenueDetail>(2)
					.ToArray();

				var venues = response.Item1
					.GetRecords<Entity>(3)
					.Select(e =>
						{
							var venue = CreateVenue(e, venueDetails, shows);
							return venue;
						})
					.ToArray();

				result = new Tuple<Venue[], string>(venues, response.Item2);
			}

			return result;
		}

		public Tuple<OrgEvent, string> GetOrgEventInfo(int id)
		{
			var request = NewRequestFactory.CreateOrgEventInfoRequest(id);
			var response = GetResponse(request);
			var result = default(Tuple<OrgEvent, string>);

			if (response != null)
			{
				var eventMetadata = response.Item1
					.GetRecords<EventMetadata>(0)
					.FirstOrDefault();

				var host = response.Item1
					.GetRecords<Entity>(1)
					.FirstOrDefault();

				var orgEvent = new OrgEvent(eventMetadata, host);

				result = new Tuple<OrgEvent, string>(orgEvent, response.Item2);
			}

			return result;
		}

		public Tuple<Org, string> GetHost(int id)
		{
			var request = NewRequestFactory.CreateHostRequest(id);
			var response = GetResponse(request);
			var result = default(Tuple<Org, string>);

			if (response != null && response.Item1.Selects != null)
			{
				var entity = response.Item1
					.GetRecords<Entity>(0)
					.FirstOrDefault();

				var eventMetadatas = response.Item1
					.GetRecords<EventMetadata>(1)
					.ToArray();

				if (entity != null)
				{
					var org = CreateOrg(entity, eventMetadatas);
					result = new Tuple<Org, string>(org, response.Item2);
				}
			}

			return result;
		}

		private Tuple<Response, string> GetResponse(Request request)
		{
			var result = default(Tuple<Response, string>);

			using (var client = new WebClient())
			{
				client.Headers[HttpRequestHeader.ContentType] = "application/json";

				try
				{
					request.ClientVersion = "2.0.0";
					var json = JsonConvert.SerializeObject(request);
					var resultString = client.UploadString(_url, json);
					result = new Tuple<Response, string>(
						JsonConvert.DeserializeObject<Response>(resultString),
						resultString);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}

			return result;
		}

		private static Venue CreateVenue(Entity entity, EventVenueDetail[] venueDetails, Show[] shows)
		{
			var venueDetail = venueDetails
				.FirstOrDefault(d => d.Venue.Id() == entity.Id);
			var venueShows = shows
				.Where(s => entity.Id == s.Venue.Id() && s.IsReference != true)
				.ToArray();

			var result = 
				new Venue(entity,
					venueDetail != null ? venueDetail.Description : null) {
				Shows = venueShows.Length > 0 ? venueShows : null
			};
			return result;
		}

		private static Org CreateOrg(Entity entity, EventMetadata[] eventMetadatas)
		{
			var orgEvents = eventMetadatas
				.Select(em => new OrgEvent(em, entity))
				.ToArray();
			var result = new Org(entity) { OrgEvents = orgEvents };
			return result;
		}
	}

	public static class ApiExtensions
	{
		public static IEnumerable<T> GetRecords<T>(this Response response, int index)
		{
			return response.Selects != null && index < response.Selects.Length 
				? (response.Selects[index].Records
					.Cast<JObject>()
					.Select(jo => jo.ToObject<T>()) ?? Enumerable.Empty<T>()) 
					: Enumerable.Empty<T>();
		}
	}
}