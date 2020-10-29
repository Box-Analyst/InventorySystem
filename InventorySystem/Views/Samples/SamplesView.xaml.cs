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
        private string empID;
        public SamplesView()
        {
            this.InitializeComponent();
        }
        //protected override void OnNavigatedTo(NavigationEventArgs e)
       // {
        //    empID = e.Parameter.ToString();
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        public string GetEmpID()
        {
            return empID;
        }
    }
}