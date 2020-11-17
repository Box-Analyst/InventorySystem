﻿using System;
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

namespace InventorySystem.Views.Settings.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangePrivilege : Page
    {
        private string empID;
        private int privLevel;
        public ChangePrivilege()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter?.ToString();
        }

        public async void ChangePrivButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeeID.Text.ToString() == "")
            {
                DisplayInputError();
            }
            else
            {
                try
                {
                    //Check if the user account exists to renew
                    if (SQL.ManageDB.CheckEmployee(int.Parse(employeeID.Text)) == true)
                    {
                        if (SQL.ManageDB.CheckAcctActive(int.Parse(employeeID.Text)) == true)
                        {
                            if (CheckIfAdmin() == false)
                            {
                                SQL.ManageDB.Update_Employee(sender, e, int.Parse(employeeID.Text), privLevel);
                                OutputSuccess.Text = "Successfully changed Employee " + employeeID.Text + "'s permissions.";
                                if (privLevel == 1)
                                {
                                    OutputSuccess2.Text = "This user now has Doctor permissions.";
                                }
                                else if (privLevel == 2)
                                {
                                    OutputSuccess2.Text = "This user now has Standard permissions.";
                                }
                                Clear();
                            }
                            else
                            {
                                Clear();
                                DisplayCannotChangeAdmin();
                            }

                        }
                        else
                        {
                            Clear();
                            DisplayUserExpired();
                        }

                    }
                    else
                    {
                        Clear();
                        DisplayUserNotFound();
                    }
                }
                catch (Exception)
                {
                    Clear();
                    ContentDialog invalidInput = new ContentDialog
                    {
                        Title = "Invalid Input",
                        Content = "Please enter your Employee ID. \n\nReminder: Employee IDs consist of \nnumeric characters only",
                        CloseButtonText = "Ok"
                    };
                    ContentDialogResult result = await invalidInput.ShowAsync();
                }
            }
        }

        private bool CheckIfAdmin()
        {
            string privEntry = SQL.ManageDB.Grab_Entries("Login", "PrivLevel", "Emp_id", employeeID.Text)[0];
            return privEntry == "0";
        }
        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsView), empID);
        }
        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb?.Name == "DoctorChoice")
            {
                privLevel = 1;
            }
            else if (rb?.Name == "StandardChoice")
            {
                privLevel = 2;
            }
            else { privLevel = 5; }
        }

        private async void DisplayInputError()
        {
            ContentDialog inputError = new ContentDialog
            {
                Title = "Invalid Account Creation",
                Content = "One or more fields are empty. Please enter \ninformation into all fields and try again.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await inputError.ShowAsync();
        }
        private async void DisplayUserExpired()
        {
            ContentDialog displayUserExpiredError = new ContentDialog
            {
                Title = "Invalid Account Modification",
                Content = "User Account is expired.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await displayUserExpiredError.ShowAsync();
        }

        private async void DisplayUserNotFound()
        {
            ContentDialog userNotFoundError = new ContentDialog
            {
                Title = "Invalid Account Modification",
                Content = "User Account does not exist.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await userNotFoundError.ShowAsync();
        }
        private async void DisplayCannotChangeAdmin()
        {
            ContentDialog displayAdminError = new ContentDialog
            {
                Title = "Invalid Account Modification",
                Content = "Permission changes cannot be made to Admin accounts.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await displayAdminError.ShowAsync();
        }
        public void Clear()
        {
            employeeID.Text = "";
        }
    }
}
