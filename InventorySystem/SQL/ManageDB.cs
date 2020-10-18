using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Windows.UI.Xaml;

namespace InventorySystem.SQL
{
    static class ManageDB
    {
        // Method to initialize database
        public static void InitializeDB()
        {
            //SqliteEngine.UseWinSqlite3(); //Configuring library to use SDK version of SQLite
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db")) //Name of .db file doesn't matter, but should be consistent across all SqliteConnection objects
            {
                db.Open(); //Open connection to database
                const string tableCommand = "CREATE TABLE IF NOT EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY AUTOINCREMENT, Text_Entry NVARCHAR(2048) NULL)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                try
                {
                    createTable.ExecuteReader(); //Execute command, throws SqliteException error if command doesn't execute properly
                }
                catch (SqliteException e)
                {
                    //Do nothing
                }
            }
        }

        // Method to grab entries from the SQLite database
        public static List<string> Grab_Entries(string search)
        {
            List<string> entries = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT Text_Entry from MyTable", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    //Handle error
                    return entries;
                }
                while (query.Read())
                {
                    var tmp = query.GetString(0);
                    if (tmp.Contains(search))
                    {
                        entries.Add(tmp);
                    }
                }
                db.Close();
            }
            return entries;
        }

        // Method to insert text into the SQLite database
        public static void Add_Text(object sender, RoutedEventArgs e, string inputVal)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);"
                };
                insertCommand.Parameters.AddWithValue("@Entry", inputVal);
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
        }

        // Method to grab Text_Entry column from MyTable table in SQLite database
        public static List<String> Grab_Entries_col()
        {
            List<string> entries = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT Text_Entry from MyTable", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    //Handle error
                    return entries;
                }
                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }
                db.Close();
            }
            return entries;
        }
    }
}
