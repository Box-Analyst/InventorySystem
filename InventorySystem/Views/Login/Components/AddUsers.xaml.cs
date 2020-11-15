﻿using InventorySystem.Views.Settings;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem.Views.Login.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddUsers : Page
    {
        private string empID;
        public AddUsers()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter?.ToString();
        }
        public void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordHash hash = new PasswordHash(password.Password);
            //If checkEmployee is false (user doesn't exist), create user account
            if (SQL.ManageDB.CheckEmployee(int.Parse(employeeID.Text)) == false)
            {
                //If checkPassword is true (password and retyped password matches), continue with account creation)
                if (password.Password == passwordRetype.Password)
                {
                    hash.SetHash();
                    var hashedPW = hash.GetHash();
                    var salt = hash.GetSalt();
                    SQL.ManageDB.Add_Employee(sender, e, int.Parse(employeeID.Text), salt, hashedPW, true, DateTime.Now);
                    OutputSuccess.Text = "Successfully added Employee " + employeeID.Text + " to the list of authorized users for this application.";
                    Clear();
                }
                else
                {
                    Clear();
                    DisplayAddUserPasswordError();
                }
            }
            else
            {
                Clear();
                DisplayAddUserError();
            }

        }
        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsView), empID);
        }

        private async void DisplayAddUserPasswordError()
        {
            ContentDialog addUserPasswordError = new ContentDialog
            {
                Title = "Invalid Account Creation",
                Content = "Passwords do not match. Please try again.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addUserPasswordError.ShowAsync();
        }

        private async void DisplayAddUserError()
        {
            ContentDialog addUserError = new ContentDialog
            {
                Title = "Invalid Account Creation",
                Content = "User Account already exists.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addUserError.ShowAsync();
        }

        private void password_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                AddUserButton_Click(sender, e);
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
