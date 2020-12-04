using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using net.reidemeister.wp.CalendarTemplates.Resources;
#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
#else
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;
#endif
using System.Reflection;
using Microsoft.Phone.Tasks;

namespace net.reidemeister.wp.CalendarTemplates
{
    public partial class HelpPage : PhoneApplicationPage
    {
        private const string SMALL = "DonationSmall";
        private const string MEDIUM = "DonationMedium";
        private const string BIG = "DonationBig";

        public HelpPage()
        {
            InitializeComponent();
#if DEBUG
            SetupMockIAP();
#endif
        }

#if DEBUG
        private void SetupMockIAP()
        {
            if (MockIAP.Initialized) return;
            MockIAP.Init();
            MockIAP.RunInMockMode(true);
            MockIAP.SetListingInformation(1, "en-us", "bla", "0.00", "Calendar Templates");
            ProductListing p = new ProductListing
            {
                Name = SMALL,
                ProductId = SMALL,
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Description = SMALL,
                FormattedPrice = "0.99"
            };
            MockIAP.AddProductListing(p.ProductId, p);
            p = new ProductListing
            {
                Name = MEDIUM,
                ProductId = MEDIUM,
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Description = MEDIUM,
                FormattedPrice = "1.99"
            };
            MockIAP.AddProductListing(p.ProductId, p);
            p = new ProductListing
            {
                Name = BIG,
                ProductId = BIG,
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Description = BIG,
                FormattedPrice = "6.99"
            };
            MockIAP.AddProductListing(p.ProductId, p);
        }
#endif

        public static string GetVersion()
        {
            return (System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(false)
                .OfType<AssemblyFileVersionAttribute>()
                .First().Version);
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.Version.Text = AppResources.ApplicationTitle + " (" + AppResources.Version + GetVersion() + ")";
            await UpdateLicense();
        }

        private async System.Threading.Tasks.Task UpdateLicense()
        {
            try
            {
                ListingInformation li = await CurrentApp.LoadListingInformationAsync();
                foreach (string key in li.ProductListings.Keys)
                {
                    ProductListing pListing = li.ProductListings[key];
                    bool active = CurrentApp.LicenseInformation.ProductLicenses[key].IsActive;
                    if (SMALL.Equals(key))
                    {
                        UpdateButton(DonationSmall, pListing, active);
                    }
                    else if (MEDIUM.Equals(key))
                    {
                        UpdateButton(DonationMedium, pListing, active);
                    }
                    else if (BIG.Equals(key))
                    {
                        UpdateButton(DonationBig, pListing, active);
                    }
                }
            }
            catch (Exception)
            {
                // whatever
            }
        }

        private void UpdateButton(Button button, ProductListing pListing, bool active)
        {
            if (active)
            {
                button.IsEnabled = false;
                button.Content = AppResources.Thanks;
            }
            else
            {
                button.IsEnabled = true;
                button.Content = pListing.FormattedPrice;
                DonationDescr.Visibility = System.Windows.Visibility.Visible;
                DonationButtons.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void DonationSmall_Click(object sender, RoutedEventArgs e)
        {
            this.Buy(SMALL);
        }

        private void DonationMedium_Click(object sender, RoutedEventArgs e)
        {
            this.Buy(MEDIUM);
        }

        private void DonationBig_Click(object sender, RoutedEventArgs e)
        {
            this.Buy(BIG);
        }

        private async void Buy(string productKey)
        {
            ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();
            string productID = li.ProductListings[productKey].ProductId;
            await Store.CurrentApp.RequestProductPurchaseAsync(productID, false);
            await this.UpdateLicense();
        }

        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask t = new MarketplaceReviewTask();
            t.Show();
        }
    }
}