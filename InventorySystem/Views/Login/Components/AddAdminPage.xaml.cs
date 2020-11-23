using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem.Views.Login.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddAdminPage : Page
    {
        private string empID;
        public AddAdminPage()
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
            if (employeeID.Text.ToString() == "" || password.Password.ToString() == "")
            {
                DisplayInputError();
            }
            else
            {
                try
                {
                        if (password.Password == passwordRetype.Password)
                        {
                                hash.SetHash();
                                var hashedPW = hash.GetHash();
                                var salt = hash.GetSalt();
                                SQL.ManageDB.Add_Employee(sender, e, int.Parse(employeeID.Text), salt, hashedPW, true, DateTime.Now, 0);
                                Frame.Navigate(typeof(LoginWindow));
                        }
                        else
                        {
                            DisplayAddUserPasswordError();
                        }
                }
                catch (Exception)
                {
                    Clear();
                    DisplayInputError();
                }
            }
        }

        private async void DisplayInputError()
        {
            ContentDialog inputError = new ContentDialog
            {
                Title = "Invalid Account Creation",
                Content = "One or more fields are empty or incorrect. Please enter \ninformation into all fields and try again.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await inputError.ShowAsync();
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


        private void Password_KeyUp(object sender, KeyRoutedEventArgs e)
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

