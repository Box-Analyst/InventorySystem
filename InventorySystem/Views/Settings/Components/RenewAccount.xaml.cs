using InventorySystem.Views.Login;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem.Views.Settings.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RenewAccount : Page
    {
        private string empID, typeOfPage;
        private List<string> passedVars = new List<string>();

        public RenewAccount()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passedVars.Clear();
            passedVars = e.Parameter as List<string>;
            empID = passedVars[0];
            typeOfPage = passedVars[1];
            if (typeOfPage == "renew")
            {
                HeaderText.Text = "Renew Account";
                ButtonText.Content = "Renew Account";
                ButtonText.Click += RenewButton_Click;
            }
            else
            {
                if (IsAdmin() == false)
                {
                    employeeID.Text = empID;
                }
                HeaderText.Text = "Reset Password";
                ButtonText.Content = "Reset Password";
                ButtonText.Click += ResetButton_Click;
            }
        }

        private bool IsAdmin()
        {
            string privLevel = SQL.ManageDB.Grab_Entries("Login", "PrivLevel", "Emp_id", empID)[0];
            return privLevel == "0";
        }

        public void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordHash hash = new PasswordHash(password.Password);
            if (employeeID.Text.ToString() == "" || password.Password.ToString() == "")
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
                        if (SQL.ManageDB.CheckAcctActive(int.Parse(employeeID.Text)) == false)
                        {
                            if (password.Password == passwordRetype.Password)
                            {
                                hash.SetHash();
                                var hashedPW = hash.GetHash();
                                var salt = hash.GetSalt();
                                SQL.ManageDB.Update_Employee(sender, e, int.Parse(employeeID.Text), salt, hashedPW, true, DateTime.Now);
                                OutputSuccess.Text = "Successfully renewed Employee " + employeeID.Text + ".";
                                OutputSuccess2.Text = "This employee now has access to this application.";
                                Clear();
                            }
                            else
                            {
                                Clear();
                                DisplayPasswordCheckError();
                            }
                        }
                        else
                        {
                            Clear();
                            DisplayUserNotExpired();
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
                    DisplayInputError();
                }
            }
        }

        public void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            //Checks if Admin is trying to change passwords, if true, allow changes to all empID numbers
            if (IsAdmin() == true)
            {
                PasswordHash hash = new PasswordHash(password.Password);
                if (employeeID.Text.ToString() == "" || password.Password.ToString() == "")
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
                            if (password.Password == passwordRetype.Password)
                            {
                                hash.SetHash();
                                var hashedPW = hash.GetHash();
                                var salt = hash.GetSalt();
                                SQL.ManageDB.Update_Employee(sender, e, int.Parse(employeeID.Text), salt, hashedPW, true, DateTime.Now);
                                OutputSuccess.Text = "Successfully reset Employee " + employeeID.Text + "'s password.";
                                Clear();
                            }
                            else
                            {
                                string temp = employeeID.Text;
                                Clear();
                                DisplayPasswordCheckError();
                                employeeID.Text = temp;
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
                        DisplayInputError();
                    }
                }
            }
            //Checks if a nonAdmin is trying to change passwords, only allows change if employee is changing their own password
            else
            {
                PasswordHash hash = new PasswordHash(password.Password);
                if (employeeID.Text.ToString() == "" || password.Password.ToString() == "")
                {
                    DisplayInputError();
                }
                else
                {
                    try
                    {
                        if (employeeID.Text == empID)
                        {
                            if (password.Password == passwordRetype.Password)
                            {
                                hash.SetHash();
                                var hashedPW = hash.GetHash();
                                var salt = hash.GetSalt();
                                SQL.ManageDB.Update_Employee(sender, e, int.Parse(employeeID.Text), salt, hashedPW, true, DateTime.Now);
                                OutputSuccess.Text = "Password reset successfully!";
                                Clear();
                            }
                            else
                            {
                                string temp = employeeID.Text;
                                Clear();
                                DisplayPasswordCheckError();
                                employeeID.Text = temp;
                            }
                        }
                        else
                        {
                            Clear();
                            DisplayNoPrivilege();
                        }
                    }
                    catch (Exception)
                    {
                        Clear();
                        DisplayInputError();
                    }
                }
            }
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsView), empID);
        }

        private async void DisplayInputError()
        {
            ContentDialog inputError = new ContentDialog
            {
                Title = "Invalid Account Modification",
                Content = "One or more fields are empty or incorrect. Please enter \ninformation into all fields and try again.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await inputError.ShowAsync();
        }

        private async void DisplayPasswordCheckError()
        {
            ContentDialog displayPasswordError = new ContentDialog
            {
                Title = "Invalid Account Modification",
                Content = "Passwords do not match. Please try again.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await displayPasswordError.ShowAsync();
        }

        private async void DisplayUserNotExpired()
        {
            ContentDialog displayPasswordError = new ContentDialog
            {
                Title = "Invalid Account Modification",
                Content = "User Account is not expired.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await displayPasswordError.ShowAsync();
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

        private async void DisplayNoPrivilege()
        {
            ContentDialog displayPasswordError = new ContentDialog
            {
                Title = "Invalid Account Modification",
                Content = "You do not have authorization to change account passwords other \nthan the password associated with your account.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await displayPasswordError.ShowAsync();
        }

        private void Password_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (typeOfPage == "renew")
                {
                    RenewButton_Click(sender, e);
                }
                else
                {
                    ResetButton_Click(sender, e);
                }
            }
        }

        public void Clear()
        {
            employeeID.Text = "";
            password.Password = "";
            passwordRetype.Password = "";
        }
    }
}