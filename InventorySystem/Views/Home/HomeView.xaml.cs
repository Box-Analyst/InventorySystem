using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using InventorySystem.Views.Admin;
using InventorySystem.Views.Notifications;
using InventorySystem.Views.Samples;

namespace InventorySystem.Views.Home
{
    public sealed partial class HomeView : Page
    {
        private string empID;
        public HomeView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter.ToString();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NotifyFrame.Navigate(typeof(Components.NotifyPane), GetEmpID());
            SamplesFrame.Navigate(typeof(SamplesView), GetEmpID());
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public string GetEmpID()
        {
            return empID;
        }
    }
}
