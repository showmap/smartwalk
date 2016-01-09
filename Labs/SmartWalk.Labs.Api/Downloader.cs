using System;
using System.IO;

namespace SmartWalk.Labs.Api
{
	public static class Downloader
	{
		private const string Root = "responses";

		public static void Run(string url)
		{
			var service = new ApiService(url);
			Console.WriteLine("Start downloading...");

			try
			{
				var indexEvents = service.GetOrgEvents();
				WriteFile(Root + "/index.json", indexEvents.Item2);
				Console.WriteLine("index.json updated.");

				foreach (var indexEvent in indexEvents.Item1)
				{
					var host = service.GetHost(indexEvent.OrgId);
					WriteFile(Root + "/orgs/org-" + indexEvent.OrgId + ".json", host.Item2);
					Console.WriteLine("Org " + indexEvent.OrgId + " json updated.");

					foreach (var orgEvent in host.Item1.OrgEvents)
					{
						var eventInfo = service.GetOrgEventInfo(orgEvent.Id);
						WriteFile(Root + "/events/" + orgEvent.Id + "/info.json", eventInfo.Item2);
						Console.WriteLine("Event " + orgEvent.Id + " info.json updated.");

						var eventShows = service.GetOrgEvent(orgEvent.Id);
						WriteFile(Root + "/events/" + orgEvent.Id + "/event.json", eventShows.Item2);
						Console.WriteLine("Event " + orgEvent.Id + " event.json updated.");

						var eventVenues = service.GetOrgEventVenues(orgEvent.Id);
						WriteFile(Root + "/events/" + orgEvent.Id + "/venues.json", eventVenues.Item2);
						Console.WriteLine("Event " + orgEvent.Id + " venues.json updated.");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Downloading Failed.");
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Console.WriteLine("Downloading finished!");
			}
		}

		private static void WriteFile(string path, string content)
		{
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			}

			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			using (var sw = new StreamWriter(path))
			{
				sw.Write(content);
			}
		}
	}
}