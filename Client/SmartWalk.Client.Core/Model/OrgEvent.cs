using System;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Client.Core.Model
{
    public class OrgEvent
    {
        private const string CantGetPropertyException = "Can't get property value because Host is not set.";

        private readonly EventMetadata _eventMetadata;
        private readonly Entity _host;
        private readonly Venue[] _venues;

        public OrgEvent(
            EventMetadata eventMetadata,
            Entity host = null,
            Venue[] venues = null)
        {
            if (eventMetadata == null) throw new ArgumentNullException("eventMetadata");

            _eventMetadata = eventMetadata;
            _host = host;
            _venues = venues;

            if (_eventMetadata.VenueTitleFormatType ==
                VenueTitleFormatType.NameAndNumber)
            {
                for (var i = 0; i < venues.Length; i++)
                {
                    var venue = venues[i];
                    venue.Number = i + 1;
                }
            }
        }

        internal EventMetadata EventMetadata
        {
            get { return _eventMetadata; }
        }

        internal Entity Host
        {
            get { return _host; }
        }

        public int Id
        {
            get { return _eventMetadata.Id; }
        }

        public int OrgId
        {
            get { return _eventMetadata.Host.Id(); }
        }

        public string Title 
        {
            get 
            { 
                if (_host == null) 
                    throw new InvalidOperationException(CantGetPropertyException);

                return _eventMetadata.Title ?? _host.Name; 
            }
        }

        public string Picture 
        {
            get 
            {
                if (_host == null) 
                    throw new InvalidOperationException(CantGetPropertyException);

                return _eventMetadata.Picture ?? _host.Picture; 
            }
        }

        public string Description 
        {
            get { return _eventMetadata.Description; }
        }

        public DateTime? StartTime 
        {
            get { return _eventMetadata.StartTime; }
        }

        public DateTime? EndTime 
        {
            get { return _eventMetadata.EndTime; }
        }

        public double? Latitude 
        {
            get { return _eventMetadata.Latitude; }
        }

        public double? Longitude 
        {
            get { return _eventMetadata.Longitude; }
        }

        public Venue[] Venues
        {
            get { return _venues; }
        }

        public override bool Equals(object obj)
        {
            var oe = obj as OrgEvent;
            if (oe != null)
            {
                var result = Equals(_eventMetadata, oe._eventMetadata) &&
                    Equals(_host, oe._host) &&
                    _venues.EnumerableEquals(oe._venues);
                return result;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCode(_eventMetadata.GetHashCode())
                    .CombineHashCode(_host.GetHashCode())
                    .CombineHashCodeOrDefault(Venues);
        }

        public override string ToString()
        {
            return string.Format(
                "Id={0}, VenuesCount={1}", 
                Id, 
                Venues != null ? Venues.Length : 0);
        }
    }
}