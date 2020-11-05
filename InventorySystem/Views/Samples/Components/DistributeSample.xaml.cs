using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem.Views.Samples.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DistributeSample : Page
    {
        private string empID;
        private List<string> passedVars = new List<string>();
        public DistributeSample()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passedVars.Clear();
            passedVars = e.Parameter as List<string>;
            empID = passedVars?[0];
            LotNumBox.Text = passedVars?[1] ?? string.Empty;
            NameAndDosageBox.Text = passedVars?[2] ?? string.Empty;
        }
        private void Add_Sample(object sender, RoutedEventArgs e)
        {
            if (SQL.ManageDB.Add_Sample(sender, e, LotNumBox.Text, NameAndDosageBox.Text, Int32.Parse(CountBox.Text), ExpirationDateBox.Text, false))
            {
                Console.WriteLine("Success!");
            }
            //Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }

        public string GetEmpID()
        {
            return empID;
        }
    }
}
