using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Import Microsoft.Data.Sqlite namespaces
using Microsoft.Data.Sqlite;

namespace InventorySystem.Views.Admin
{
    public sealed partial class AdminView : Page
    {
        public AdminView()
        {
            this.InitializeComponent();
            Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }

        private void Add_Text(object sender, RoutedEventArgs e)
        {
            SQL.ManageDB.Add_Text(sender, e, Input_Box.Text);
            Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }
    }
}
