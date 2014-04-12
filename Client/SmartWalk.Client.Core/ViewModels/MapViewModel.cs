using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.ViewModels
{
    public class MapViewModel : ActiveViewModel
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IShowDirectionsTask _showDirectionsTask;

        private Parameters _parameters;
        private MapAnnotation _annotation;
        private MvxCommand<Address> _showDirectionsCommand;

        public MapViewModel(
            IAnalyticsService analyticsService,
            IShowDirectionsTask showDirectionsTask) : base(analyticsService)
        {
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

        public ICommand ShowDirectionsCommand
        {
            get
            {
                if (_showDirectionsCommand == null)
                {
                    _showDirectionsCommand = new MvxCommand<Address>(
                        address =>
                        {
                            _showDirectionsTask.ShowDirections(address);

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShowDirections);
                        },
                        address => address != null);
                }

                return _showDirectionsCommand;
            }
        }

        protected override object InitParameters
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
                    parameters.Number,
                    parameters.Addresses.Items);
            }
            else
            {
                Annotation = null;
            }
        }

        public class Parameters
        {
            public int Number { get; set; }
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
            int number,
            Address[] addresses)
        {
            Number = number;
            Title = title;
            Addresses = addresses;
        }

        public int Number { get; private set; }
        public string Title { get; private set; }
        public Address[] Addresses { get; private set; }
    }
}