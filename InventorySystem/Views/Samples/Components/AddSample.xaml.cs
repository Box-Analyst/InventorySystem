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
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;

namespace InventorySystem.Views.Samples.Components
{
    public sealed partial class AddSample : Page
    {
        private string empID;
        public AddSample()
        {
            this.InitializeComponent();
        }
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    empID = e.Parameter.ToString();
        //}

        private void Add_Sample(object sender, RoutedEventArgs e)
        {
            if (SQL.ManageDB.Add_Sample(sender, e, LotNumBox.Text, NameAndDosageBox.Text, Int32.Parse(CountBox.Text), ExpirationDateBox.Text, false))
            {
                Console.WriteLine("Success!");

                // Replace with navigate to sample view for new sample?
                // or show a success message and close on user acknoledgement
                Window.Current.Close();
            }
            else
            {
                // show an error
            }
            //Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }

        public string GetEmpID()
        {
            return empID;
        }

    }
}
