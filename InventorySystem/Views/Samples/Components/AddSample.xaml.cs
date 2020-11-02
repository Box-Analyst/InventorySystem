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
        private string LogMode;
        public AddSample()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter.ToString();
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
                            if (SQL.ManageDB.Add_Sample(sender, e, LotNumBox.Text, NameAndDosageBox.Text, Int32.Parse(CountBox.Text), ExpirationDateBox.Text, isExpired))
                            {
                                if (LogMode == "ADD")
                                {
                                    SQL.ManageDB.Add_Log(sender, e, empID, LotNumBox.Text, DateTime.Now.ToString(), "NULL", "NULL", LogMode);
                                    OutputSuccess.Text = "Successfully added " + CountBox.Text + " units of " + LotNumBox.Text;
                                    Clear();
                                }
                                else if (LogMode == "RECEIVE")
                                {
                                    if (SQL.ManageDB.Check_RepID_RegEx(RepID.Text))
                                    {
                                        SQL.ManageDB.Add_Log(sender, e, empID, LotNumBox.Text, DateTime.Now.ToString(), "NULL", RepID.Text, LogMode);
                                        OutputSuccess.Text = "Successfully recieved " + CountBox.Text + " units of " + LotNumBox.Text + " from " + RepID.Text;
                                        Clear();
                                    } else
                                    {
                                        DisplayRepIDError();
                                    }
                                }
                                else
                                {
                                    DisplayModeError();
                                }
                            }
                        }
                        else
                        {
                            DisplayExpirationDateError();
                        }
                    }
                    else
                    {
                        DisplayCountError();
                    }
                }
                else
                {
                    DisplayNameAndDosageError();
                }
            } else
            {
                DisplayLotNumberError();
            }
        }

        public string GetEmpID()
        {
            return empID;
        }

        private async void DisplayExpirationDateError()
        {
            ContentDialog addExpirationDateError = new ContentDialog
            {
                Title = "Invalid Expiration Date Input",
                Content = "Expiration Date formatted Incorrectly or empty. \nFormatting should be MM/DD/YYYY",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addExpirationDateError.ShowAsync();
        }

        private async void DisplayLotNumberError()
        {
            ContentDialog addLotNumberError = new ContentDialog
            {
                Title = "Invalid Lot Number Input",
                Content = "Lot Number formatted Incorrectly or empty. \nFormatting should be alphanumeric or numbers and letters only",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addLotNumberError.ShowAsync();
        }

        private async void DisplayNameAndDosageError()
        {
            ContentDialog addNameAndDosageError = new ContentDialog
            {
                Title = "Invalid Name and Dosage Input",
                Content = "Name and Dosage formatted Incorrectly or empty. \nMake sure you have a dosage!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addNameAndDosageError.ShowAsync();
        }

        private async void DisplayCountError()
        {
            ContentDialog addCountError = new ContentDialog
            {
                Title = "Invalid Count Input",
                Content = "Count is formatted Incorrectly or empty. \nOnly insert integers!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addCountError.ShowAsync();
        }

        private async void DisplayModeError()
        {
            ContentDialog addModeError = new ContentDialog
            {
                Title = "No Mode Selected!",
                Content = "Please Select a Mode and try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addModeError.ShowAsync();
        }

        private async void DisplayRepIDError()
        {
            ContentDialog addRepIDError = new ContentDialog
            {
                Title = "Invalid Representative ID Input",
                Content = "Representative ID is formatted Incorrectly or empty. \nFormatting should be alphanumeric, or numbers and letters only!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addRepIDError.ShowAsync();
        }
        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if(rb.Name == "ManualButton")
            {
                LogMode = "ADD";
                RepIDTitle.Visibility = Visibility.Collapsed;
                RepID.Visibility = Visibility.Collapsed;
                RepIDWhiteSpace.Visibility = Visibility.Collapsed;
            } else if(rb.Name == "ReceiveButton")
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
