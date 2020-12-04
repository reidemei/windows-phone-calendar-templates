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
using System.ComponentModel;
using Microsoft.Xna.Framework.GamerServices;
using System.Windows.Input;
using System.Resources;

namespace net.reidemeister.wp.CalendarTemplates.Pages
{
    public partial class EditTemplatePage : PhoneApplicationPage
    {
        private CalendarTemplate original;
        private CalendarTemplate template;

        public EditTemplatePage()
        {
            InitializeComponent();
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Save;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.Cancel;
            #region Icons
            Icon.Items.Add("anchor.png");
            Icon.Items.Add("at.png");
            Icon.Items.Add("baby.png");
            Icon.Items.Add("bath.png");
            Icon.Items.Add("bed.png");
            Icon.Items.Add("beer.png");
            Icon.Items.Add("book.open.png");
            Icon.Items.Add("box.png");
            Icon.Items.Add("cabinet.files.png");
            Icon.Items.Add("calculator.png");
            Icon.Items.Add("camera.png");
            Icon.Items.Add("cards.club.png");
            Icon.Items.Add("cards.diamond.png");
            Icon.Items.Add("cards.heart.png");
            Icon.Items.Add("cards.spade.png");
            Icon.Items.Add("cart.png");
            Icon.Items.Add("cd.png");
            Icon.Items.Add("chat.png");
            Icon.Items.Add("chess.rook.png");
            Icon.Items.Add("church.png");
            Icon.Items.Add("city.png");
            Icon.Items.Add("clean.png");
            Icon.Items.Add("clipboard.edit.png");
            Icon.Items.Add("clock.png");
            Icon.Items.Add("console.png");
            Icon.Items.Add("control.guide.png");
            Icon.Items.Add("controller.xbox.png");
            Icon.Items.Add("cup.full.png");
            Icon.Items.Add("cup.paper.png");
            Icon.Items.Add("cup.png");
            Icon.Items.Add("cupcake.png");
            Icon.Items.Add("database.png");
            Icon.Items.Add("delete.png");
            Icon.Items.Add("draw.pen.png");
            Icon.Items.Add("draw.pencil.png");
            Icon.Items.Add("email.minimal.png");
            Icon.Items.Add("film.png");
            Icon.Items.Add("food.png");
            Icon.Items.Add("forklift.png");
            Icon.Items.Add("gas.png");
            Icon.Items.Add("gift.png");
            Icon.Items.Add("globe.wire.png");
            Icon.Items.Add("group.png");
            Icon.Items.Add("home.png");
            Icon.Items.Add("iphone.png");
            Icon.Items.Add("key.old.png");
            Icon.Items.Add("laptop.png");
            Icon.Items.Add("magnify.png");
            Icon.Items.Add("man.suitcase.run.png");
            Icon.Items.Add("map.folds.png");
            Icon.Items.Add("music.png");
            Icon.Items.Add("page.copy.png");
            Icon.Items.Add("people.png");
            Icon.Items.Add("phone.png");
            Icon.Items.Add("plane.rotated.45.png");
            Icon.Items.Add("projector.png");
            Icon.Items.Add("run.png");
            Icon.Items.Add("sailboat.png");
            Icon.Items.Add("scale.png");
            Icon.Items.Add("suitcase.png");
            Icon.Items.Add("timer.png");
            Icon.Items.Add("tool.png");
            Icon.Items.Add("train.png");
            Icon.Items.Add("transit.bus.png");
            Icon.Items.Add("transit.car.png");
            Icon.Items.Add("truck.png");
            Icon.Items.Add("tv.png");
            Icon.Items.Add("weight.png");
            Icon.Items.Add("windowsphone.png");
            #endregion
            #region Reminder
            for (int i = 0; i <= 9; i++) 
            {
                this.Reminder.Items.Add(AppResources.ResourceManager.GetString("Reminder" + i, AppResources.Culture));
            }
            
            #endregion
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                // passiert, wenn der ListPicker eine ganze Seite anzeigt
                return;
            }
            if (this.NavigationContext.QueryString.ContainsKey("template"))
            {
                string templateID = this.NavigationContext.QueryString["template"];
                this.original = (from t in App.Model.Templates where t.ID.Equals(templateID) select t).FirstOrDefault();
                this.PageTitle.Text = AppResources.EditTitle;
            }
            else
            {
                this.original = new CalendarTemplate();
                this.PageTitle.Text = AppResources.AddTitle;
            }
            this.template = new CalendarTemplate();
            this.template.CopyFrom(this.original);
            this.DataContext = this.template;
        }

        private static void UpdateBoundText()
        {
            var focusObj = FocusManager.GetFocusedElement();
            if (focusObj == null) return;
            var binding = focusObj is TextBox ? ((TextBox)focusObj).GetBindingExpression(TextBox.TextProperty)
                        : focusObj is PasswordBox ? ((PasswordBox)focusObj).GetBindingExpression(PasswordBox.PasswordProperty)
                        : null;
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }

        private bool ConfirmUnsavedChanges()
        {
            UpdateBoundText();
            if (this.template.Equals(this.original)) return (true);
            return (MessageBox.Show(AppResources.EditConfirmExitDescr, AppResources.EditConfirmExitTitle, MessageBoxButton.OKCancel) == 
                MessageBoxResult.OK);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (!this.ConfirmUnsavedChanges())
            {
                e.Cancel = true;
            }
        }

        private async void Save_Click(object sender, EventArgs e)
        {
            UpdateBoundText();
            if (this.original.ID == null)
            {
                // we have a new one
                this.original.ID = Guid.NewGuid().ToString();
                App.Model.Templates.Add(this.original);
            }
            this.original.CopyFrom(this.template);
            await this.original.Save();
            if (this.original.HasTitle)
            {
                this.original.UpdateTile();
            }
            this.NavigationService.GoBack();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (this.ConfirmUnsavedChanges())
            {
                this.NavigationService.GoBack();
            }
        }
    }
}