using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem.Views.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SamplesView : Page
    {
        public SamplesView()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                var url = "ms-appx:///Assets/Wide310x150Logo.scale-200.png";
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(url));

                Thickness margin = new Thickness(5, 0, 5, 10);

                GridViewItem gvi = new GridViewItem
                {
                    //MaxWidth = 500,
                    MaxWidth = (Frame.ActualWidth / 2) - 10,
                    Margin = margin,
                    Background = new SolidColorBrush(Windows.UI.Colors.Gray),
                    //Content = "GridViewItem " + i
                    Content = img
                };

                //SampleList.Items.Add(img);
                SampleList.Items.Add(gvi);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Change GridViewItem's MaxWidth on window resize
            //GridViewItem.Items.MaxWidth = (Frame.ActualWidth / 2) - 10;
        }
    }
}
