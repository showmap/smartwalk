using System;
using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class VenueShow
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Description { get; set; }

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
    }
}