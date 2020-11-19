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

        private void Distribute_Sample(object sender, RoutedEventArgs e)
        {
            if (SQL.ManageDB.Check_Count_RegEx(DisAmountBox.Text))
            {
                if (SQL.ManageDB.Check_RepID_RegEx(PatientIDBox.Text))
                {
                    if (SQL.ManageDB.Update_Sample(sender, e, LotNumBox.Text, -Int32.Parse(DisAmountBox.Text)))
                    {
                        SQL.ManageDB.Add_Log(sender, e, empID, LotNumBox.Text, DateTime.Now.ToString(), PatientIDBox.Text, "NULL", "DISTRIBUTE");
                        DistributeButton.Visibility = Visibility.Collapsed;
                        ContinueButton.Visibility = Visibility.Visible;
                        OutputSuccess.Text = "Successfully Distributed " + DisAmountBox.Text + " Units of " + LotNumBox.Text + " to " + PatientIDBox.Text;
                        Clear();
                    }
                    else
                    {
                        OutputSuccess.Text = "Failed to Distribute!";
                    }
                }
                else
                {
                    DisplayError("Invalid Patient ID Input", "Patient ID is formatted Incorrectly or empty. \nFormatting should be alphanumeric, or numbers and letters only!");
                }
            }
            else
            {
                DisplayError("Invalid Distribution Amount Input", "Distribution Amount is formatted Incorrectly or empty. \nOnly insert integers!");
            }
            //Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }

        public string GetEmpID()
        {
            return empID;
        }

        private async void DisplayError(string title, string content)
        {
            ContentDialog addError = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addError.ShowAsync();
        }

        public void Clear()
        {
            LotNumBox.Text = "";
            NameAndDosageBox.Text = "";
            DisAmountBox.Text = "";
            PatientIDBox.Text = "";
        }

        private void NavBack(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Samples.SamplesView), empID);
        }
    }
}