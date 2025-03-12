using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FolderHabits
{
    public class ActivityTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool hasActivity)
            {
                return hasActivity
                    ? new SolidColorBrush(Colors.White)
                    : new SolidColorBrush(Color.FromRgb(85, 85, 85));
            }

            return new SolidColorBrush(Color.FromRgb(85, 85, 85));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}