using System;
using Cirrious.CrossCore.Converters;
using System.Globalization;

namespace SmartWalk.Client.Core.Converters
{
    public class ValueConverter<TInput> : IMvxValueConverter 
        where TInput : class
    {
        private readonly Func<TInput, object> _convertFunc;

        public ValueConverter(Func<TInput, object> convertFunc)
        {
            _convertFunc = convertFunc;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = value as TInput;
            if (t != null || value == null)
            {
                return _convertFunc(t);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}