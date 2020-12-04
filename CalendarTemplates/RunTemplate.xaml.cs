using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Storage;
using net.reidemeister.wp.CalendarTemplates.Data;
using net.reidemeister.wp.CalendarTemplates.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace net.reidemeister.wp.CalendarTemplates
{
    public partial class RunTemplate : PhoneApplicationPage
    {
        public RunTemplate()
        {
            InitializeComponent();
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.AllTemplates;
            LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
                //throw new ExitException();
            }
            LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;

            if (this.NavigationContext.QueryString.ContainsKey("template"))
            {
                string templateID = this.NavigationContext.QueryString["template"];
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var dataFolder = await local.GetFolderAsync("templates");
                var file = await dataFolder.GetFileAsync(templateID + ".xml");
                CalendarTemplate template = await CalendarTemplate.Load(dataFolder, file);
                template.Run();

                Uri uri = (Uri)new StringToUriValueConverter().Convert(template.Icon, null, null, null);
                BitmapImage img = new BitmapImage(uri);
                this.Icon.Source = img;
                LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Back_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", System.UriKind.Relative));
        }
    }
}