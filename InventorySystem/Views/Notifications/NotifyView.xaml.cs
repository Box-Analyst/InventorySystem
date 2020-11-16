using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using InventorySystem.Views.Settings;
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
            ConstructExpireSoonList();
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

            if (entries.Count > 0)
            {
                ExpiredListTitle.Visibility = Visibility.Visible;
                ResolveExpiredButton.Visibility = Visibility.Visible;
                ExpiredList.Visibility = Visibility.Visible;
            }

            return entries;
        }

        // Method for getting a list of expired samples lot numbers
        private List<string> ExpiryListLot()
        {
            List<string> entries = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT LotNum FROM Sample WHERE isExpired = 1", db);
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

            if (entries.Count > 0)
            {
                ExpiredListTitle.Visibility = Visibility.Visible;
                ResolveExpiredButton.Visibility = Visibility.Visible;
                ExpiredList.Visibility = Visibility.Visible;
            }

            return entries;
        }

        // Method for grabbing a list of expiring soon samples
        private List<string> ExpireSoonList()
        {
            List<string> entries = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT NameandDosage FROM Sample WHERE isExpired = 0", db);
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

            if (entries.Count > 0) ExpireSoonFrame.Visibility = Visibility.Visible;
            return entries;
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

        // Method for constructing list of samples expiring soon
        private void ConstructExpireSoonList()
        {
            var samples = ExpireSoonList();
            int count = 0;
            if (SQL.ManageDB.IsEmpty())
            {
                return;
            }
            ColumnDefinition sampleCol = new ColumnDefinition();
            sampleCol.Width = new GridLength(10, GridUnitType.Star);
            ExpiringListGrid.ColumnDefinitions.Add(sampleCol);

            foreach (string sample in samples)
            {
                RowDefinition sampleRow = new RowDefinition();
                ExpiringListGrid.RowDefinitions.Add(sampleRow);
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

                ExpiringListGrid.Children.Add(sampleButton);
                count++;
            }
        }

        // Method for getting page width
        private static double GetWidth()
        {
            return ((Frame)Window.Current.Content).ActualWidth;

        }

        private async void ResolveExpiredButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentDialog areYouSure = new ContentDialog
            {
                Title = "Delete Expired?",
                Content = "This action will delete all expired samples, and cannot be undone.",
                PrimaryButtonText = "Cancel",
                CloseButtonText = "Delete"

            };

            ContentDialogResult result = await areYouSure.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                var samples = ExpiryListLot();
                foreach (var sample in samples)
                {
                    SQL.ManageDB.Add_Log(sender, e, "test", sample, DateTime.Now.ToString(CultureInfo.CurrentCulture), "NULL", "NULL", "DELETE");
                }
                SQL.ManageDB.Delete_Expired(sender, e);
            }
            Frame.Navigate(typeof(NotifyView));
        }
    }
}
