using System.Linq;
using SmartWalk.Labs.Api.DataContracts;

namespace SmartWalk.Labs.Api.Model
{
    public class GroupedShow : Show
    {
        public Show[] Shows { get; private set; }

        public GroupedShow(Show[] shows)
        {
            Shows = shows;

            Venue = shows.SelectMany(s => s.Venue).Distinct().ToArray();

            var anyShow = shows.First();
            Title = anyShow.Title;
            Description = anyShow.Description;
            Picture = anyShow.Picture;
            Pictures = anyShow.Pictures;
            DetailsUrl = anyShow.DetailsUrl;
        }
    }
}