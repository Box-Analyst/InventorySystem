using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

namespace InventorySystem.Views.Home.Components
{
    public sealed partial class NotifyPane : Page
    {
        public NotifyPane()
        {
            this.InitializeComponent();
            expiryList();
            expireSoonList();
        }

        private void expiryList()
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
                    return;
                }
                while (query.Read())
                {
                    var tmp = query.GetString(0);
                    entries.Add(tmp);
                }
                db.Close();
            }
            int expiryListCount = entries.Count - 1;
            expiredAlert.Text = entries[0] + " and " + expiryListCount + " more samples are expired.";
        }

        private void expireSoonList()
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
                    if (SQL.ManageDB.Check_ExpiresSoon(entryExpiryDate[i], 30))
                    {
                        entriesExpireSoon.Add(entries[i]);
                    }
                }
            }
            int expiryListCount = entriesExpireSoon.Count - 1;
            expiredAlert.Text = entriesExpireSoon[0] + " and " + expiryListCount + " more samples are expiring soon.";
        }
    }
}
