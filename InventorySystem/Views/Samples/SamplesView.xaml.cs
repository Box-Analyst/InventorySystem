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

namespace InventorySystem.Views.Samples
{
    public sealed partial class SamplesView : Page
    {
        public SamplesView()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Placeholder elements
            for (int i = 0; i < 20; i++)
            {
                Thickness margin = new Thickness(5, 0, 5, 10);
                Thickness padding = new Thickness(10);
                GridViewItem gvimg = new GridViewItem
                {
                    MaxWidth = (Frame.ActualWidth / 2) - 12,
                    Margin = margin,
                    Padding = padding,
                    Background = new SolidColorBrush(Windows.UI.Colors.Gray),
                    Content = "SampleContent"
                };
                SampleList.Items.Add(gvimg);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) { }
    }
}
