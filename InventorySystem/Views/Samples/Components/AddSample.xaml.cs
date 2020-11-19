using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

//using Windows.UI.WindowManagement;

namespace InventorySystem.Views.Samples.Components
{
    public sealed partial class AddSample : Page
    {
        private string empID, LogMode;
        private List<string> passedVars = new List<string>();

        public AddSample()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passedVars.Clear();
            if (e.Parameter is string)
            {
                empID = e.Parameter.ToString();
            }
            else
            {
                passedVars = e.Parameter as List<string>;
                empID = passedVars?[0];
                LotNumBox.Text = passedVars?[1] ?? string.Empty;
                NameAndDosageBox.Text = passedVars?[2] ?? string.Empty;
                //Debug.WriteLine(LotNumBox.Text);
            }
        }

        private void Add_Sample(object sender, RoutedEventArgs e)
        {
            bool isExpired = false;
            if (SQL.ManageDB.Check_LotNumber_RegEx(LotNumBox.Text))
            {
                if (SQL.ManageDB.Check_NameAndDosage_RegEx(NameAndDosageBox.Text))
                {
                    if (SQL.ManageDB.Check_Count_RegEx(CountBox.Text))
                    {
                        if (SQL.ManageDB.Check_ExpirationDate_RegEx(ExpirationDateBox.Text))
                        {
                            if (SQL.ManageDB.Check_IsExpired(ExpirationDateBox.Text))
                            {
                                isExpired = true;
                            }
                            if (SQL.ManageDB.Add_Sample(sender, e, LotNumBox.Text, NameAndDosageBox.Text, int.Parse(CountBox.Text), ExpirationDateBox.Text, isExpired))
                            {
                                if (LogMode == "ADD")
                                {
                                    SQL.ManageDB.Add_Log(sender, e, empID, LotNumBox.Text, DateTime.Now.ToString(CultureInfo.CurrentCulture), "NULL", "NULL", LogMode);
                                    OutputSuccess.Text = "Successfully added " + CountBox.Text + " units of " + LotNumBox.Text;
                                    Clear();
                                }
                                else if (LogMode == "RECEIVE")
                                {
                                    if (SQL.ManageDB.Check_RepID_RegEx(RepID.Text))
                                    {
                                        SQL.ManageDB.Add_Log(sender, e, empID, LotNumBox.Text, DateTime.Now.ToString(CultureInfo.CurrentCulture), "NULL", RepID.Text, LogMode);
                                        OutputSuccess.Text = "Successfully recieved " + CountBox.Text + " units of " + LotNumBox.Text + " from " + RepID.Text;
                                        Clear();
                                    }
                                    else
                                    {
                                        DisplayError("Invalid Representative ID Input", "Representative ID is formatted Incorrectly or empty. \nFormatting should be alphanumeric, or numbers and letters only!");
                                    }
                                }
                                else
                                {
                                    DisplayError("No Mode Selected", "Please Select a Mode and try again!");
                                }
                            }
                        }
                        else
                        {
                            DisplayError("Invalid Expiration Date Input", "Expiration Date formatted Incorrectly or empty. \nFormatting should be MM/DD/YYYY");
                        }
                    }
                    else
                    {
                        DisplayError("Invalid Count Input", "Count is formatted Incorrectly or empty. \nOnly insert integers!");
                    }
                }
                else
                {
                    DisplayError("Invalid Name and Dosage Input", "Name and Dosage formatted Incorrectly or empty. \nMake sure you have a dosage!");
                }
            }
            else
            {
                DisplayError("Invalid Lot Number Input", "Lot Number formatted Incorrectly or empty. \nFormatting should be alphanumeric or numbers and letters only");
            }
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

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb?.Name == "ManualButton")
            {
                LogMode = "ADD";
                RepIDTitle.Visibility = Visibility.Collapsed;
                RepID.Visibility = Visibility.Collapsed;
                RepIDWhiteSpace.Visibility = Visibility.Collapsed;
            }
            else if (rb?.Name == "ReceiveButton")
            {
                LogMode = "RECEIVE";
                RepIDTitle.Visibility = Visibility.Visible;
                RepID.Visibility = Visibility.Visible;
                RepIDWhiteSpace.Visibility = Visibility.Visible;
            }
        }

        public void Clear()
        {
            LotNumBox.Text = "";
            NameAndDosageBox.Text = "";
            CountBox.Text = "";
            ExpirationDateBox.Text = "";
            RepID.Text = "";
        }
    }
}