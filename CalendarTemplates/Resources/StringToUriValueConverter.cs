using System;
#if DEBUG
using System.Diagnostics;
#endif
using System.Globalization;
using System.Windows.Data;

namespace net.reidemeister.wp.CalendarTemplates.Resources
{
    public class StringToUriValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string prefix = "/Assets/Icons/" + (Themes.CurrentTheme() == Theme.Dark ? "Dark" : "Light") + "/";
            return (new Uri(value== null ? "" : prefix + value.ToString(), UriKind.Relative));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
