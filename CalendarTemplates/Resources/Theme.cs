using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace net.reidemeister.wp.CalendarTemplates.Resources
{
    public enum Theme 
    { 
        Dark, Light
    }

    public class Themes
    {
        public static Theme CurrentTheme()
        {
            if ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible) 
            {
                return (Theme.Dark);
            }
            return (Theme.Light);
        }
    }
}
