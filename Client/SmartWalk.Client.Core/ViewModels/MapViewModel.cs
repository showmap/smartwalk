using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using Cirrious.CrossCore.Core;

namespace SmartWalk.Client.Core.ViewModels
{
    public class MapViewModel : ActiveViewModel, IShareableViewModel
    {
        private readonly IEnvironmentService _environmentService;
        private readonly IAnalyticsService _analyticsService;

        private Parameters _parameters;
        private MapAnnotation _annotation;
        private MapType _currentMapType = MapType.Standard;
        private MvxCommand _showDirectionsCommand;
        private MvxCommand  _copyAddressCommand;
        private MvxCommand _switchMapTypeCommand;
        private MvxCommand _shareCommand;

        public MapViewModel(
            IEnvironmentService environmentService,
            IAnalyticsService analyticsService) : base(analyticsService)
        {
            _environmentService = environmentService;
            _analyticsService = analyticsService;
        }

        public event EventHandler<MvxValueEventArgs<string>> Share;

        public MapAnnotation Annotation
        {
            get
            {
                return _annotation;
            }
            private set
            {
                if (!Equals(_annotation, value))
                {
                    _annotation = value;
                    RaisePropertyChanged(() => Annotation);
                }
            }
        }

        public MapType CurrentMapType
        {
            get
            {
                return _currentMapType;
            }
            private set
            {
                if (_currentMapType != value)
                {
                    _currentMapType = value;
                    RaisePropertyChanged(() => CurrentMapType);
                }
            }
        }

        public ICommand CopyAddressCommand
        {
            get
            {
                if (_copyAddressCommand == null)
                {
                    _copyAddressCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCopyAddress);

                            var address = GetAddressText();
                            _environmentService.Copy(address);
                        },
                        () => 
                            Annotation != null &&
                            Annotation.Addresses != null);
                }

                return _copyAddressCommand;
            }
        }

        public ICommand ShareCommand
        {
            get
            {
                if (_shareCommand == null)
                {
                    _shareCommand = new MvxCommand(() =>
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShare);

                            var address = GetAddressText();
                            if (address != null && Share != null)
                            {
                                Share(this, new MvxValueEventArgs<string>(address));
                            }
                        },
                        () => 
                            Annotation != null &&
                            Annotation.Addresses != null);
                }

                return _shareCommand;
            }
        }

        public ICommand ShowDirectionsCommand
        {
            get
            {
                if (_showDirectionsCommand == null)
                {
                    _showDirectionsCommand = new MvxCommand(
                        () =>
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShowDirections);

                            var address = Annotation.Addresses.FirstOrDefault();
                            _environmentService.ShowDirections(address);
                        },
                        () => 
                            Annotation != null &&
                            Annotation.Addresses != null);
                }

                return _showDirectionsCommand;
            }
        }

        public ICommand SwitchMapTypeCommand
        {
            get
            {
                if (_switchMapTypeCommand == null)
                {
                    _switchMapTypeCommand = new MvxCommand(
                        () =>
                        {
                            CurrentMapType = CurrentMapType.GetNextMapType();

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                CurrentMapType == MapType.Standard 
                                ? Analytics.ActionLabelSwitchMapToStandard
                                : (CurrentMapType == MapType.Satellite
                                    ? Analytics.ActionLabelSwitchMapToSatellite
                                    : Analytics.ActionLabelSwitchMapToHybrid));
                        });
                }

                return _switchMapTypeCommand;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            if (parameters != null)
            {
                Annotation = new MapAnnotation(
                    parameters.Title.GetAbbreviation(2),
                    parameters.Title,
                    parameters.Addresses.Items);
            }
            else
            {
                Annotation = null;
            }
        }

        public class Parameters : ParametersBase
        {
            public string Title { get; set; }
            public Addresses Addresses { get; set; }

            public override bool Equals(object obj)
            {
                var parameters = obj as Parameters;
                if (parameters != null)
                {
                    return Equals(Location, parameters.Location) &&
                        Equals(Title, parameters.Title) &&
                        Equals(Addresses, parameters.Addresses);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Initial
                    .CombineHashCodeOrDefault(Location)
                    .CombineHashCodeOrDefault(Title)
                    .CombineHashCodeOrDefault(Addresses);
            }
        }

        private string GetAddressText()
        {
            var address = 
                Annotation != null && Annotation.Addresses != null
                ? Annotation.Addresses
                    .Select(a => a.AddressText)
                    .FirstOrDefault()
                : null;
            return address;
        }
    }

    public class Addresses
    {
        public Address[] Items { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class AddressesParser : MvxStringToTypeParser.IExtraParser
    {
        public bool Parses(Type t)
        {
            return t == typeof(Addresses);
        }

        public object ReadValue(Type t, string input, string fieldOrParameterName)
        {
            return JsonConvert.DeserializeObject(input, t);
        }
    }

    public class MapAnnotation
    {
        public MapAnnotation(
            string pin,
            string title,
            Address[] addresses)
        {
            Pin = pin;
            Title = title;
            Addresses = addresses;
        }

        public string Pin { get; private set; }
        public string Title { get; private set; }
        public Address[] Addresses { get; private set; }

        public override bool Equals(object obj)
        {
            var ma = obj as MapAnnotation;
            if (ma != null)
            {
                return Title == ma.Title;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Title)
                .CombineHashCodeOrDefault(Addresses);
        }
    }
}