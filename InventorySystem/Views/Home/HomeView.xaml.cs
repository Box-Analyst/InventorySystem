using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using InventorySystem.Views.Shell;
using Microsoft.Data.Sqlite;

namespace InventorySystem.Views.Home
{
    public sealed partial class HomeView : Page
    {
        private string empID;
        public HomeView()
        {
            this.InitializeComponent();
            ExpiryList();
            ExpireSoonList();
            ConstructExpiredList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter?.ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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
            int expiryListCount = entries.Count - 1;
            if (expiryListCount < 0)
            {
                ExpiredFrame.Visibility = Visibility.Collapsed;
                ExpiredList.Visibility = Visibility.Collapsed;
            }
            else if (expiryListCount == 0) ExpiredAlert.Text = entries[0] + " is expired.";
            else ExpiredAlert.Text = entries[0] + " and " + expiryListCount + " more samples are expired.";

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
            int expiryListCount = entriesExpireSoon.Count - 1;
            if (expiryListCount < 0)
            {
                ExpireSoonFrame.Visibility = Visibility.Collapsed;
                ExpireSoonButton.Visibility = Visibility.Collapsed;
            }
            else if (expiryListCount == 0) ExpireSoonAlert.Text = entriesExpireSoon[0] + " is expiring soon.";
            else ExpireSoonAlert.Text = entriesExpireSoon[0] + " and " + expiryListCount + " more samples are expiring soon.";
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
            ColumnDefinition recCol = new ColumnDefinition();
            sampleCol.Width = new GridLength(10, GridUnitType.Star);
            ExpiryListGrid.ColumnDefinitions.Add(sampleCol);

            for (int i = 1; i < samples.Count; i++)
            {
                RowDefinition sampleRow = new RowDefinition();
                ExpiryListGrid.RowDefinitions.Add(sampleRow);
                sampleRow.Height = GridLength.Auto;

                Button sampleButton = new Button
                {
                    Name = "sampleButton" + count,
                    Content = samples[i],
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

        private void ExpireSoonButton_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ResolveExpiredButton_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
