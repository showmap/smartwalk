using System;
using SmartWalk.Core.Utils;
using SmartWalk.Core.Model.Interfaces;
using System.Collections.Generic;

namespace SmartWalk.Core.Model
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
                return (Start != DateTime.MinValue ? " " + Start.ToShortTimeString() : string.Empty) + 
                    (End != DateTime.MaxValue ? " " + End.ToShortTimeString() : string.Empty) +
                    (Site != null ? " " + Site.SearchableText : string.Empty) +
                        (Description != null ? " " + Description :  string.Empty);
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