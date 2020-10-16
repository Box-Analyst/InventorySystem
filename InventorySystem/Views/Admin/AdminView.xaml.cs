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

        //private void Add_Text(object sender, RoutedEventArgs e)
        //{
        //    SQL.ManageDB.Add_Text(sender, e);
        //    Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        //}

        // Method to insert text into the SQLite database
        private void Add_Text(object sender, RoutedEventArgs e)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                //Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", Input_Box.Text);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    //Handle error
                    return;
                }
                db.Close();
            }
            Output.ItemsSource = SQL.ManageDB.Grab_Entries_col();
        }
    }
}
