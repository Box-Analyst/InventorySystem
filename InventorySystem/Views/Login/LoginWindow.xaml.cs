﻿using InventorySystem.Views.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
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

        }

        //Upon Clicking Login, user is sent to the Main Page of the Application.
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string hashedPW;
            int empID = int.Parse(employeeID.Text);
            PasswordHash hash = new PasswordHash(password.Password);
            hash.SetHash();
            hashedPW = hash.GetHash();

            try
            {
                if (SQL.ManageDB.CheckPassword(hashedPW, empID))
                {
                    this.Frame.Navigate(typeof(Views.Shell.MainNavView), employeeID.Text);
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
            catch (Exception ex)
            {
                if (ex != null)
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
                else { };
            }


        }
    
        //Clears the Employee ID and Password fields
        public void Clear()
        {
            employeeID.Text = "";
            password.Password = "";
        }
       // private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
       // {
       //     this.Frame.Navigate(typeof(Views.Login.Components.AddUsers));
       // }

    }
}
