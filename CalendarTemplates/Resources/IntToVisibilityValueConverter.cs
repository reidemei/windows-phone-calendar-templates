using System;
#if DEBUG
using System.Diagnostics;
#endif
using System.Globalization;
using System.Windows.Data;

namespace net.reidemeister.wp.CalendarTemplates.Resources
{
    public class IntToVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return (System.Windows.Visibility.Collapsed);
            string[] parameters = parameter.ToString().Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in parameters)
            {
                try
                {
                    int? p = int.Parse(s.ToString());
                    if ((value as int?) == p) return (System.Windows.Visibility.Visible);
                }
                catch (Exception)
                {
#if DEBUG
                    if (Debugger.IsAttached) Debugger.Break();
#endif
                }
            }
            return (System.Windows.Visibility.Collapsed);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
