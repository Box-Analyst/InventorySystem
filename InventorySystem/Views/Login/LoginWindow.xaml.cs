using InventorySystem.Views.Login;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginWindow : Page
    {
        public LoginWindow()
        {
            this.InitializeComponent();
            SQL.ManageDB.SetAcctsExpired();
        }

        //Upon Clicking Login, user is sent to the Main Page of the Application.
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PasswordHash hash = new PasswordHash(password.Password);
                string salt = SQL.ManageDB.Grab_Entries("Login", "Salt", "Emp_id", employeeID.Text)[0];
                hash.SetHash(salt);
                string hashedPW = hash.GetHash();
                int empID = int.Parse(employeeID.Text);
                if (SQL.ManageDB.CheckPassword(hashedPW, empID))
                {
                    if (SQL.ManageDB.CheckAcctActive(empID))
                    {
                        SQL.ManageDB.Update_IsExpired();
                        SQL.ManageDB.UpdateAcctLoggedIn(empID);
                        this.Frame.Navigate(typeof(Views.Shell.MainNavView), empID);
                    }
                    else
                    {
                        Clear();
                        ContentDialog expiredAcct = new ContentDialog
                        {
                            Title = "Expired User Account",
                            Content = "The account associated with this Employee ID has expired. \nPlease contact your local Admin account holder to get your account reactivated. \n\n(Note: Accounts expire after 90 days of inactivity)",
                            CloseButtonText = "Ok"
                        };
                        ContentDialogResult result = await expiredAcct.ShowAsync();
                    }

                }
                else
                {
                    Clear();
                    ContentDialog invalidLogin = new ContentDialog
                    {
                        Title = "Invalid Employee ID/Password",
                        Content = "The Employee ID/Password entered is incorrect. \nPlease try again. \n\n(Hint: Check CAPS/NUM LOCK)",
                        CloseButtonText = "Ok"
                    };
                    ContentDialogResult result = await invalidLogin.ShowAsync();
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

        private void password_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
        //Clears the Employee ID and Password fields
        public void Clear()
        {
            employeeID.Text = "";
            password.Password = "";
        }
    }
}
