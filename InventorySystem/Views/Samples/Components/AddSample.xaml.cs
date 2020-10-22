using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
//using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;

namespace InventorySystem.Views.Samples.Components
{
    public sealed partial class AddSample : Page
    {
        public AddSample()
        {
            this.InitializeComponent();
        }

        private void Add_Sample(object sender, RoutedEventArgs e)
        {
            if(SQL.ManageDB.Add_Sample(sender, e, LotNumBox.Text, NameAndDosageBox.Text, Int32.Parse(CountBox.Text), ExpirationDateBox.Text, false))
            {
                Console.WriteLine("Success!");
            }
            //Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }
    }
}
