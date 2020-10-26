using InventorySystem.Views.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string hashedPW;
            PasswordHash hash = new PasswordHash(password.Password);
            hash.SetHash();
            hashedPW = hash.GetHash();

            if (SQL.ManageDB.CheckPassword(hashedPW, int.Parse(employeeID.Text)))
            {
                this.Frame.Navigate(typeof(Views.Shell.MainNavView));
            }


        }
        private void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.Login.Components.AddUsers));
        }


    }
}
