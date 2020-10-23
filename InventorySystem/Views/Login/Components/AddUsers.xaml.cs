﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
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

namespace InventorySystem.Views.Login.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddUsers : Page
    {
        public AddUsers()
        {
            this.InitializeComponent();
        }

        public void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordHash hash = new PasswordHash(password.Password);
            string hashedPW;
            //If checkEmployee is false (user doesn't exist), create user account
            if (SQL.ManageDB.CheckEmployee(int.Parse(employeeID.Text)) == false)
            {
                //If checkPassword is true (password and retyped password matches), continue with account creation)
                if (password.Password == passwordRetype.Password)
                {
                    hash.SetHash();
                    hashedPW = hash.GetHash();
                    SQL.ManageDB.Add_Employee(sender, e, int.Parse(employeeID.Text), hashedPW, "True");
                    this.Frame.Navigate(typeof(LoginWindow));
                }



            }

        }

    }
}
