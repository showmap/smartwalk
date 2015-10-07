using Cirrious.CrossCore.Core;

namespace SmartWalk.Client.Core.Utils
{
    public class ValueChangedEventArgs<T> : MvxValueEventArgs<T>
    {
        public ValueChangedEventArgs(T previousValue, T value) : base(value)
        {
            PreviousValue = previousValue;
        }

        public T PreviousValue { get; private set; }
    }
}