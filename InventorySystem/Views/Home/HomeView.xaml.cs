using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

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
            // Placeholder elements
            for (int i = 0; i < 20; i++)
            {
                var url = "ms-appx:///Assets/Wide310x150Logo.scale-200.png";
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri(url))
                };
                Thickness margin = new Thickness(5, 0, 5, 10);
                GridViewItem gvimg = new GridViewItem
                {
                    MaxWidth = (Frame.ActualWidth / 2) - 12,
                    Margin = margin,
                    Background = new SolidColorBrush(Windows.UI.Colors.Gray),
                    Content = img
                };
                SampleList.Items?.Add(gvimg);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) { }
    }
}
