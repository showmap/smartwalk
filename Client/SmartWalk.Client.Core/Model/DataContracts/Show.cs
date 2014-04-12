using System;
using System.Collections.Generic;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Model.DataContracts
{
    public class Show : IShow
    {
        public int Id { get; set; }
        public Reference[] Venue { get; set; }
        public bool? IsReference { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Picture { get; set; }
        public string DetailsUrl { get; set; }

        IReference[] IShow.Venue { 
            get { return Venue; } 
            set { Venue = (Reference[])value; }
        }

        public override bool Equals(object obj)
        {
            var show = obj as Show;
            if (show != null)
            {
                return Id == show.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCode(Id)
                    .CombineHashCodeOrDefault(Venue)
                    .CombineHashCode(IsReference)
                    .CombineHashCodeOrDefault(Title)
                    .CombineHashCodeOrDefault(Description)
                    .CombineHashCodeOrDefault(Picture)
                    .CombineHashCode(StartTime)
                    .CombineHashCode(EndTime)
                    .CombineHashCodeOrDefault(Picture)
                    .CombineHashCodeOrDefault(DetailsUrl);
        }

        public override string ToString()
        {
            return string.Format("Id={0}, Title={1})", Id, Title);
        }
    }

    public class ShowComparer : IComparer<Show>
    {
        public int Compare(Show x, Show y)
        {
            if (x.StartTime < y.StartTime)
            {
                return -1;
            }

            if (x.StartTime > y.StartTime)
            {
                return 1;
            }

            if (x.EndTime < y.EndTime)
            {
                return -1;
            }

            if (x.EndTime > y.EndTime)
            {
                return 1;
            }

            return 0;
        }
    }
}