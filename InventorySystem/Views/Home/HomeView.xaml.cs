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
        public HomeView()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NotifyFrame.Navigate(typeof(Components.NotifyPane));
            SamplesFrame.Navigate(typeof(SamplesView));
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
