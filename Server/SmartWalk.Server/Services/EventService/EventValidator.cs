using System.Collections.Generic;
using System.Linq;
using Orchard.Localization;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EventService
{
    public class EventValidator
    {
        private readonly Localizer _localizer;

        public EventValidator(Localizer localizer)
        {
            _localizer = localizer;
        }

        private Localizer T { get { return _localizer; } }

        // TODO: To validate if there are duplicated venues
        public IList<ValidationError> ValidateEvent(EventMetadataVm model)
        {
            var result = new List<ValidationError>();

            var titleProperty = model.GetPropertyName(p => p.Title);
            if (!string.IsNullOrEmpty(model.Title))
            {
                if (model.Title.Length > 255)
                {
                    result.Add(new ValidationError(
                                   titleProperty,
                                   T("Title can not be longer than 255 characters.").Text));
                }
            }

            var startDateProperty = model.GetPropertyName(p => p.StartDate);
            if (!model.StartDate.HasValue)
            {
                result.Add(new ValidationError(
                               startDateProperty,
                               T("Start date can not be empty.").Text));
            }

            if (model.StartDate.HasValue && model.EndDate.HasValue &&
                model.StartDate.Value > model.EndDate.Value)
            {
                result.Add(new ValidationError(
                               startDateProperty,
                               T("Start date has to be less than or equal to the end date.").Text));
            }

            var pictureProperty = model.GetPropertyName(p => p.Picture);
            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                {
                    result.Add(new ValidationError(
                                   pictureProperty,
                                   T("Picture URL can not be longer than 255 characters.").Text));
                }
                else if (!model.Picture.IsUrlValid())
                {
                    result.Add(new ValidationError(
                                   pictureProperty,
                                   T("Picture URL has bad format.").Text));
                }
            }

            var descriptionProperty = model.GetPropertyName(p => p.Description);
            if (!string.IsNullOrEmpty(model.Description))
            {
                if (model.Description.Length > 3000)
                {
                    result.Add(new ValidationError(
                                   descriptionProperty,
                                   T("Description can not be longer than 3000 characters.").Text));
                }
            }

            if (model.Host == null)
            {
                result.Add(new ValidationError(
                               model.GetPropertyName(p => p.Host),
                               T("Event organizer can not be empty.").Text));
            }

            var venuesProperty = model.GetPropertyName(p => p.Venues);
            var showsProperty = Reflection<EntityVm>.GetProperty(p => p.Shows).Name;
            var venues = model.Venues != null
                ? model.Venues.Where(v => !v.Destroy).ToArray()
                : new EntityVm[] { };

            if (model.Status == EventStatus.Public && venues.Length < 1)
            {
                result.Add(new ValidationError(
                               venuesProperty,
                               T("At least one venue is required for public events.").Text));
            }

            for (var i = 0; i < venues.Length; i++)
            {
                var venueVm = venues[i];
                if (venueVm.Id <= 0)
                {
                    result.Add(new ValidationError(
                                   string.Format("{0}[{1}]", venuesProperty, i + 1),
                                   T("The venue should be selected.").Text));
                }

                var shows = venueVm.Shows != null
                    ? venueVm.Shows.Where(v => !v.Destroy).ToArray()
                    : new ShowVm[] { };
                for (var j = 0; j < shows.Length; j++)
                {
                    var showVm = shows[j];
                    result.AddRange(ValidateShow(
                        showVm,
                        model,
                        string.Format(
                            "{0}[{1}].{2}[{3}].",
                            venuesProperty,
                            i + 1,
                            showsProperty,
                            j + 1)));
                }
            }

            return result;
        }

        private IEnumerable<ValidationError> ValidateShow(ShowVm model, EventMetadataVm eventVm, string prefix = "")
        {
            var result = new List<ValidationError>();

            var titleProperty = model.GetPropertyName(p => p.Title);
            if (string.IsNullOrEmpty(model.Title))
            {
                result.Add(new ValidationError(
                               prefix + titleProperty,
                               T("Title can not be empty!").Text));
            }
            else if (model.Title.Length > 255)
            {
                result.Add(new ValidationError(
                               prefix + titleProperty,
                               T("Title can not be larger than 255 characters!").Text));
            }

            var pictureProperty = model.GetPropertyName(p => p.Picture);
            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                {
                    result.Add(new ValidationError(
                                   prefix + pictureProperty,
                                   T("Picture url can not be larger than 255 characters!").Text));
                }
                else if (!model.Picture.IsUrlValid())
                {
                    result.Add(new ValidationError(
                                   prefix + pictureProperty,
                                   T("Picture url is in bad format!").Text));
                }
            }

            var detailsUrlProperty = model.GetPropertyName(p => p.DetailsUrl);
            if (!string.IsNullOrEmpty(model.DetailsUrl))
            {
                if (model.DetailsUrl.Length > 255)
                {
                    result.Add(new ValidationError(
                                   prefix + detailsUrlProperty,
                                   T("Details url can not be larger than 255 characters!").Text));
                }
                else if (!model.DetailsUrl.IsUrlValid())
                {
                    result.Add(new ValidationError(
                                   prefix + detailsUrlProperty,
                                   T("Details url is in bad format!").Text));
                }
            }

            var startTimeProperty = model.GetPropertyName(p => p.StartTime);
            if (model.StartTime.HasValue && model.EndTime.HasValue &&
                model.StartTime.Value > model.EndTime.Value)
            {
                result.Add(new ValidationError(
                               prefix + startTimeProperty,
                               T("Show start time has to be less than or equal to the end time.").Text));
            }

            var eventStartDate = eventVm.StartDate;
            var eventEndDate = eventVm.EndDate ?? eventVm.StartDate;

            if (model.StartTime.HasValue && eventStartDate.HasValue &&
                (model.StartTime.Value < eventStartDate.Value.AddDays(-1) ||
                (eventEndDate.HasValue && model.StartTime.Value > eventEndDate.Value.AddDays(1))))
            {
                result.Add(new ValidationError(
                               prefix + startTimeProperty,
                               T("Show start time has to be between event start and end dates.").Text));
            }

            var endTimeProperty = model.GetPropertyName(p => p.EndTime);
            if (model.EndTime.HasValue && eventStartDate.HasValue &&
                (model.EndTime.Value < eventStartDate.Value.AddDays(-1) ||
                (eventEndDate.HasValue && model.EndTime.Value > eventEndDate.Value.AddDays(2))))
            {
                result.Add(new ValidationError(
                               prefix + endTimeProperty,
                               T("Show end time has to be between event start and end dates.").Text));
            }

            return result;
        }
    }
}