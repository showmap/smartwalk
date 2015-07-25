using System;
using System.Collections.Generic;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.Model.DataContracts
{
    public class Show : IShow
    {
        public const int DayGroupId = -1000;

        public int Id { get; set; }
        public Reference[] Venue { get; set; }
        public bool? IsReference { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Picture { get; set; }
        public Pictures Pictures { get; set; }
        public string DetailsUrl { get; set; }

        public ShowStatus Status { get; set; }

        IReference[] IShow.Venue { 
            get { return Venue; } 
            set { Venue = (Reference[])value; }
        }

        public override bool Equals(object obj)
        {
            var show = obj as Show;
            if (show != null)
            {
                return Id == show.Id &&
                    Venue.EnumerableEquals(show.Venue) &&
                    IsReference == show.IsReference &&
                    Title == show.Title &&
                    Description == show.Description &&
                    StartTime == show.StartTime &&
                    EndTime == show.EndTime &&
                    Picture == show.Picture &&
                    Pictures == show.Pictures &&
                    DetailsUrl == show.DetailsUrl;
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
                    .CombineHashCodeOrDefault(Pictures)
                    .CombineHashCode(StartTime)
                    .CombineHashCode(EndTime)
                    .CombineHashCodeOrDefault(DetailsUrl);
        }

        public override string ToString()
        {
            return string.Format("Id={0}, Title={1}, StartTime={2})", Id, Title, StartTime);
        }
    }

    public class ShowComparer : IComparer<Show>
    {
        private readonly SortBy _sortBy;

        public ShowComparer(SortBy sortBy)
        {
            _sortBy = sortBy;
        }

        public int Compare(Show x, Show y)
        {
            if (_sortBy == SortBy.Time)
            {
                if (x.StartTime < y.StartTime ||
                    (!x.StartTime.HasValue && y.StartTime.HasValue))
                {
                    return -1;
                }

                if (x.StartTime > y.StartTime ||
                    (x.StartTime.HasValue && !y.StartTime.HasValue))
                {
                    return 1;
                }

                if (x.EndTime < y.EndTime ||
                    (!x.EndTime.HasValue && y.EndTime.HasValue))
                {
                    return -1;
                }

                if (x.EndTime > y.EndTime ||
                    (x.EndTime.HasValue && !y.EndTime.HasValue))
                {
                    return 1;
                }
            }
            else if (_sortBy == SortBy.Name)
            {
                return string.Compare(x.Title, y.Title, 
                    StringComparison.CurrentCulture);
            }

            return 0;
        }
    }
}