using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InventorySystem.Views.Home.Components
{
    public sealed partial class NotifyPane : Page
    {
        public NotifyPane()
        {
            this.InitializeComponent();
            ExpiryList();
            ExpireSoonList();
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
            if (expiryListCount <= 0)
            {
                ExpiredAlert.Text = "No expired samples found.";
            }
            else
            {
                ExpiredAlert.Text = entries[0] + " and " + expiryListCount + " more samples are expired.";
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
            int expiryListCount = entriesExpireSoon.Count - 1;
            if (expiryListCount < 0) ExpireSoonAlert.Text = "No samples are expiring soon.";
            else if (expiryListCount == 0) ExpireSoonAlert.Text = entriesExpireSoon[0] + " is expiring soon.";
            else ExpireSoonAlert.Text = entriesExpireSoon[0] + " and " + expiryListCount + " more samples are expiring soon.";
        }

        private void ConstructExpiredList()
        {
            var samples = ExpiryList();
            int numRows = SQL.ManageDB.NumberOfRows();
            int count = 0;
            if (numRows != 0)
            {
                ColumnDefinition sampleCol = new ColumnDefinition();
                ColumnDefinition recCol = new ColumnDefinition();
                ColumnDefinition distCol = new ColumnDefinition();
                sampleCol.Width = new GridLength(7, GridUnitType.Star);
                recCol.Width = new GridLength(1.5, GridUnitType.Star);
                distCol.Width = new GridLength(1.5, GridUnitType.Star);
                expiryListGrid.ColumnDefinitions.Add(sampleCol);
                expiryListGrid.ColumnDefinitions.Add(recCol);
                expiryListGrid.ColumnDefinitions.Add(distCol);

                foreach (string sample in samples)
                {
                    //currentSample = sample;
                    RowDefinition sampleRow = new RowDefinition();
                    expiryListGrid.RowDefinitions.Add(sampleRow);
                    sampleRow.Height = GridLength.Auto;

                    Button sampleButton = new Button
                    {
                        Name = "sampleButton" + count,
                        Content = sample,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 0, 2, 2),
                        Width = .7 * GetWidth()
                    };
                    Grid.SetRow(sampleButton, count);
                    Grid.SetColumn(sampleButton, 0);
                    //sampleButton.Click += new RoutedEventHandler(SampleButton_Click);

                    Button recButton = new Button
                    {
                        Name = "recButton" + count,
                        Content = "Receive",
                        Width = .15 * GetWidth(),
                        Margin = new Thickness(0, 0, 2, 2)
                    };
                    Grid.SetRow(recButton, count);
                    Grid.SetColumn(recButton, 1);
                    //recButton.Click += RecButton_Click;

                    Button distButton = new Button
                    {
                        Name = "distButton" + count,
                        Content = "Distribute",
                        Width = .15 * GetWidth(),
                        Margin = new Thickness(0, 0, 0, 2)
                    };
                    Grid.SetRow(distButton, count);
                    Grid.SetColumn(distButton, 2);
                    //distButton.Click += new RoutedEventHandler(DistButton_Click);

                    expiryListGrid.Children.Add(sampleButton);
                    expiryListGrid.Children.Add(recButton);
                    expiryListGrid.Children.Add(distButton);
                    count++;
                }
            }
        }

        private double GetWidth()
        {
            return ((Frame)Window.Current.Content).ActualWidth;

        }
    }
}
