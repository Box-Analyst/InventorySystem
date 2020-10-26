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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem.Views.Samples.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DistributeSample : Page
    {
        public DistributeSample()
        {
            this.InitializeComponent();
        }

        private void Add_Sample(object sender, RoutedEventArgs e)
        {
            if (SQL.ManageDB.Add_Sample(sender, e, LotNumBox.Text, NameAndDosageBox.Text, Int32.Parse(CountBox.Text), ExpirationDateBox.Text, false))
            {
                Console.WriteLine("Success!");
            }
            //Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }
    }
}
