using System;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.ViewModels
{
    public class MapViewModel : MvxViewModel
    {
        private MapAnnotation _annotation;

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

        public void Init(Parameters parameters)
        {
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
            public string Title { get; set; }
            public int Number { get; set; }
            public Addresses Addresses { get; set; }
        }
    }

    public class Addresses
    {
        public AddressInfo[] Items { get; set; }

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
            AddressInfo[] addressInfos)
        {
            Number = number;
            Title = title;
            AddressInfos = addressInfos;
        }

        public int Number { get; private set; }
        public string Title { get; private set; }
        public AddressInfo[] AddressInfos { get; private set; }
    }
}