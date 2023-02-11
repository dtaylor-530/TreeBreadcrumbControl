using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TreeBreadcrumbControl
{
    public class CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable enumerable)
            {
                List<object> objects = new();
                int start = 0;
                if (parameter is int iParam)
                {
                    start = iParam;
                }

                int i = 0;
                var x = enumerable.GetEnumerator();
                while (x.MoveNext())
                {
                    if (i++ > start)
                    {
                        objects.Add(x.Current);
                    }
                }
                return objects;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static CollectionConverter Instance { get; } = new();

    }
}
