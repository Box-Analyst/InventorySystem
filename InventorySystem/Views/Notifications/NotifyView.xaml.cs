using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Data.Sqlite;

namespace InventorySystem.Views.Notifications
{
    public sealed partial class NotifyView : Page
    {
        public NotifyView()
        {
            this.InitializeComponent();
            ExpiryList();
            ExpireSoonList();
            ConstructExpiredList();
        }

        // Method for getting a list of expired samples
        private List<string> ExpiryList()
        {
            List<string> entries = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT NameandDosage FROM Sample WHERE isExpired = 1", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine("Exception:" + error);
                    db.Close();
                    return null;
                }
                while (query.Read())
                {
                    var tmp = query.GetString(0);
                    entries.Add(tmp);
                }
                db.Close();
            }

            return entries;
        }

        // Method for grabbing a list of expiring soon samples
        private void ExpireSoonList()
        {
            List<string> entries = new List<string>();
            List<string> entryExpiryDate = new List<string>();
            List<string> entriesExpireSoon = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT NameandDosage, ExpirationDate FROM Sample WHERE isExpired = 0", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine("Exception:" + error);
                    db.Close();
                    return;
                }
                while (query.Read())
                {
                    var tmp = query.GetString(0);
                    entries.Add(tmp);
                    var tmp1 = query.GetString(1);
                    entryExpiryDate.Add(tmp1);
                }
                db.Close();

                for (int i = 0; i < entries.Count; i++)
                {
                    if (SQL.ManageDB.Check_ExpiresSoon(entryExpiryDate[i], -30))
                    {
                        entriesExpireSoon.Add(entries[i]);
                    }
                }
            }
        }

        // Method for constructing list of expired samples
        private void ConstructExpiredList()
        {
            var samples = ExpiryList();
            int count = 0;
            if (SQL.ManageDB.IsEmpty())
            {
                return;
            }
            ColumnDefinition sampleCol = new ColumnDefinition();
            sampleCol.Width = new GridLength(10, GridUnitType.Star);
            ExpiryListGrid.ColumnDefinitions.Add(sampleCol);

            foreach (string sample in samples)
            {
                RowDefinition sampleRow = new RowDefinition();
                ExpiryListGrid.RowDefinitions.Add(sampleRow);
                sampleRow.Height = GridLength.Auto;

                Button sampleButton = new Button
                {
                    Name = "sampleButton" + count,
                    Content = sample,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 2, 2),
                    Width = GetWidth()
                };
                Grid.SetRow(sampleButton, count);
                Grid.SetColumn(sampleButton, 0);

                ExpiryListGrid.Children.Add(sampleButton);
                count++;
            }
        }

        // Method for getting page width
        private static double GetWidth()
        {
            return ((Frame)Window.Current.Content).ActualWidth;

        }
    }
}
