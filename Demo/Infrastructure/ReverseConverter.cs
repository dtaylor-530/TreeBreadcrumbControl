using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Demo
{
    public class ReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<object> list = new();
            if (value is IEnumerable enumerable)
            {
                foreach (var e in enumerable)
                    list.Add(e);
                 list.Reverse();
                return list;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ReverseConverter Instance { get; } = new ReverseConverter(); 
    }
}
