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

namespace SmartWalk.Client.Core.ViewModels
{
    public class MapViewModel : ActiveViewModel
    {
        private readonly IClipboard _clipboard;
        private readonly IAnalyticsService _analyticsService;
        private readonly IShowDirectionsTask _showDirectionsTask;

        private Parameters _parameters;
        private MapAnnotation _annotation;
        private MvxCommand _showDirectionsCommand;
        private MvxCommand  _copyAddressCommand;

        public MapViewModel(
            IClipboard clipboard,
            IAnalyticsService analyticsService,
            IShowDirectionsTask showDirectionsTask) : base(analyticsService)
        {
            _clipboard = clipboard;
            _analyticsService = analyticsService;
            _showDirectionsTask = showDirectionsTask;
        }

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

        public ICommand CopyAddressCommand
        {
            get
            {
                if (_copyAddressCommand == null)
                {
                    _copyAddressCommand = new MvxCommand(() => 
                        {
                            var address = Annotation.Addresses
                                .Select(a => a.AddressText)
                                .FirstOrDefault();
                            _clipboard.Copy(address);
                        },
                        () => 
                            Annotation != null &&
                            Annotation.Addresses != null);
                }

                return _copyAddressCommand;
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
                            var address = Annotation.Addresses.FirstOrDefault();

                            _showDirectionsTask.ShowDirections(address);

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShowDirections);
                        },
                        () => 
                            Annotation != null &&
                            Annotation.Addresses != null);
                }

                return _showDirectionsCommand;
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
            string title,
            Address[] addresses)
        {
            Title = title;
            Addresses = addresses;
        }

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