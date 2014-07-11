using SmartWalk.Shared;

namespace SmartWalk.Server.ViewModels
{
    public class ShowVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // TODO: Why don't we just pass one timestamp for Start and End?
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }

        public string Picture { get; set; }
        public string DetailsUrl { get; set; }

        // TODO: Why do we need these for?
        public string StartDateTime {
            get { return string.Concat(StartDate, " ", StartTime); }
        }

        public string EndDateTime {
            get { return string.Concat(EndDate, " ", EndTime); }
        }

        [UsedImplicitly]
        public bool Destroy { get; set; }
    }
}