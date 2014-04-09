using System;
using System.Collections.Generic;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.Interfaces;

namespace SmartWalk.Client.Core.Model
{
    public class VenueShow : ISearchable
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Description { get; set; }

        public string Logo { get; set; }

        public WebSiteInfo Site { get; set; }

        public string SearchableText
        {
            get
            {
                return (Start != DateTime.MinValue ? " " + Start : string.Empty) + 
                    (End != DateTime.MaxValue ? " " + End : string.Empty) +
                    (Site != null ? " " + Site.SearchableText : string.Empty) +
                        (Description != null ? " " + Description :  string.Empty);
            }
        }

        public VenueShowStatus Status
        {
            get 
            {
                var status = 
                    (Start.Date != DateTime.Now.Date) ||
                        (Start == DateTime.MinValue && End >= DateTime.Now) ||
                            (End == DateTime.MaxValue) ||
                                (End >= DateTime.Now)
                        ? VenueShowStatus.NotStarted 
                        : VenueShowStatus.Finished;

                if (Start.Date == DateTime.Now.Date &&
                    Start <= DateTime.Now &&
                    DateTime.Now <= End)
                {
                    status = VenueShowStatus.Started;
                }

                return status;
            }
        }

        public override bool Equals(object obj)
        {
            var show = obj as VenueShow;
            if (show != null)
            {
                return Start == show.Start &&
                    End == show.End &&
                    Description == show.Description;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(Start)
                .CombineHashCode(End)
                .CombineHashCodeOrDefault(Description);
        }

        public VenueShow Clone()
        {
            return new VenueShow {
                Start = Start,
                End = End,
                Description = Description,
                Logo = Logo,
                Site = Site
            };
        }
    }

    public enum VenueShowStatus
    {
        NotStarted,
        Started,
        Finished
    }

    public class VenueShowComparer : IComparer<VenueShow>
    {
        public int Compare(VenueShow x, VenueShow y)
        {
            if (x.Start < y.Start)
            {
                return -1;
            }

            if (x.Start > y.Start)
            {
                return 1;
            }

            if (x.End < y.End)
            {
                return -1;
            }

            if (x.End > y.End)
            {
                return 1;
            }

            return 0;
        }
    }
}