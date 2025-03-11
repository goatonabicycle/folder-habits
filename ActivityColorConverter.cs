using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FolderHabits
{
    public class ActivityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool hasActivity)
            {                
                return hasActivity
                    ? new SolidColorBrush(Color.FromRgb(57, 211, 83))  
                    : new SolidColorBrush(Color.FromRgb(235, 237, 240));
            }

            return new SolidColorBrush(Color.FromRgb(235, 237, 240));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}