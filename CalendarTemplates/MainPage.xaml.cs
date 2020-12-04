using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using net.reidemeister.wp.CalendarTemplates.Resources;
using net.reidemeister.wp.CalendarTemplates.Data;
using System.Diagnostics;
using Windows.Storage;

namespace net.reidemeister.wp.CalendarTemplates.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.Model;

            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Add;
            (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.Help;
#if DEBUG
            ApplicationBarMenuItem menu = new ApplicationBarMenuItem();
            menu.Text = "add debug items";
            menu.Click += Debug_Click;
            this.ApplicationBar.MenuItems.Add(menu);
#endif
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // always remove all entries from the stack
            while (this.NavigationService.CanGoBack) NavigationService.RemoveBackEntry();

            if (!App.Model.IsDataLoaded) await App.Model.LoadData();
        }

        private void Help_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", System.UriKind.Relative));
        }

        private void Add_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditTemplatePage.xaml", System.UriKind.Relative));
        }

        private void TemplatesList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!((sender as Grid).DataContext is CalendarTemplate)) return;
            CalendarTemplate template = (sender as Grid).DataContext as CalendarTemplate;
            template.Run();
        }

        private void TemplatesList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var m = ContextMenuService.GetContextMenu(sender as DependencyObject);
            if (!((sender as Grid).DataContext is CalendarTemplate)) return;
            CalendarTemplate template = (sender as Grid).DataContext as CalendarTemplate;
            (m.Items[2] as MenuItem).Header = template.HasTitle ? AppResources.Unpin : AppResources.Pin;
            if (m != null) m.IsOpen = true;
        }

        private void TemplatesList_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };
        }

        private void TemplatesList_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }

        private void EditMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as MenuItem).DataContext is CalendarTemplate)) return;
            CalendarTemplate template = (sender as MenuItem).DataContext as CalendarTemplate;
            NavigationService.Navigate(new Uri("/EditTemplatePage.xaml?template=" + HttpUtility.UrlEncode(template.ID), System.UriKind.Relative));
        }

        private async void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as MenuItem).DataContext is CalendarTemplate)) return;
            CalendarTemplate template = (sender as MenuItem).DataContext as CalendarTemplate;
            App.Model.Templates.Remove(template);
#if DEBUG
            Debug.WriteLine("removing file '" + template.ID + "' for deleted template '" + template.Name + "'");
#endif
            if (template.HasTitle)
            {
                template.DeleteTile();
            }
            try
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var dataFolder = await local.GetFolderAsync("templates");
                var file = await dataFolder.GetFileAsync(template.ID + ".xml");
                await file.DeleteAsync();
            }
            catch (Exception)
            {
                // egal
            }
        }

        private void PinMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as MenuItem).DataContext is CalendarTemplate)) return;
            CalendarTemplate template = (sender as MenuItem).DataContext as CalendarTemplate;
            if (template.HasTitle)
            {
                template.DeleteTile();
            }
            else
            {
                template.CreateTile();
            }
        }

#if DEBUG
        private void Debug_Click(object sender, EventArgs e)
        {
            App.Model.Templates.Add (
                new CalendarTemplate()
                {
                    ID = "sample1",
                    Icon = "forklift.png",
                    Name = "Work",
                    Subject = "Work",
                    Location = "Pier 21",
                    TimeEndOffsetHours = 8
                });
            App.Model.Templates.Add(
                new CalendarTemplate()
                {
                    ID = "sample3",
                    Icon = "group.png",
                    Name = "Team Meeting",
                    Subject = "Team Meeting",
                    Location = "Office 3",
                    TimeEndOffsetHours = 0,
                    TimeEndOffsetMinutes = 30,
                    Status = 1
                });
            App.Model.Templates.Add(
                new CalendarTemplate()
                {
                    ID = "sample2",
                    Icon = "cup.png",
                    Name = "Coffee Break",
                    Subject = "Coffee Break",
                    Location = "Coffee Shop",
                    TimeEndOffsetHours = 0,
                    TimeEndOffsetMinutes = 15
                });
            App.Model.Templates.Add(
                new CalendarTemplate()
                {
                    ID = "sample5",
                    Icon = "clock.png",
                    Name = "Wakeup Call",
                    Reminder = 1
                });
            App.Model.Templates.Add(
                new CalendarTemplate()
                {
                    ID = "sample6",
                    Icon = "weight.png",
                    Name = "Workout",
                    Subject = "Workout",
                    Location = "Gym",
                });
            App.Model.Templates.Add(
                new CalendarTemplate()
                {
                    ID = "sample4",
                    Icon = "cart.png",
                    Name = "Shopping",
                    Status = 4
                });
            App.Model.Templates.Add(
                new CalendarTemplate()
                {
                    ID = "sample6",
                    Icon = "film.png",
                    Name = "Movie Evening",
                    Subject = "Movie Evening",
                    Location = "Home",
                    Time = 1,
                    Reminder = 3,
                    FixedStartTime = new DateTime(1, 1, 1, 19, 0, 0),
                    TimeEndOffsetHours = 4
                });
        }
#endif

    }
}