using System.Linq;
using SmartWalk.Server.Models.DataContracts;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Services.QueryService
{
    public class QueryMigrations
    {
        public void Migrate(Request request, Response response)
        {
            // TODO: To remove in the long run, since new (non-mobile) clients may not be aware of this
            // version before 2.0, fixing the client bug with datetime casting (1 hour gap)
            if (string.IsNullOrWhiteSpace(request.ClientVersion))
            {
                foreach (var select in response.Selects)
                {
                    var shows = select.Records.OfType<Show>();
                    foreach (var show in shows)
                    {
                        if (show.StartTime.HasValue)
                        {
                            show.StartTime = show.StartTime.Value.AddHours(-1);
                        }

                        if (show.EndTime.HasValue)
                        {
                            show.EndTime = show.EndTime.Value.AddHours(-1);
                        }
                    }
                }
            }
        }
    }
}