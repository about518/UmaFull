using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace UmaFull
{
    public class BoolToStringConverter : BoolToValueConverter<string>
    {
        public BoolToStringConverter()
        {
            TrueValue = "★";
            FalseValue = "　";
        }
    }

    public class BoolToValueConverter<T> : IValueConverter
    {
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? FalseValue : ((bool)value ? TrueValue : FalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && EqualityComparer<T>.Default.Equals((T)value, TrueValue);
        }
    }
}
