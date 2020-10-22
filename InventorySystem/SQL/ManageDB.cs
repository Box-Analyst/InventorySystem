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
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db")) //Name of .db file doesn't matter, but should be consistent across all SqliteConnection objects
            {
                db.Open(); //Open connection to database
                
                string tableCommand = "CREATE TABLE IF NOT EXISTS Login (Emp_id NUMERIC PRIMARY KEY NOT NULL UNIQUE, Pin VARCHAR (6) NOT NULL, IsActive BOOLEAN NOT NULL)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                try
                {
                    createTable.ExecuteReader(); //Execute command, throws SqliteException error if command doesn't execute properly
                }
                catch (SqliteException e)
                {
                    Console.WriteLine("Table error: " + e);
                }
                tableCommand = "CREATE TABLE IF NOT EXISTS Sample (LotNum VARCHAR PRIMARY KEY NOT NULL UNIQUE, NameandDosage VARCHAR(50) NOT NULL, Count INTEGER NOT NULL, ExpirationDate DATE NOT NULL, isExpired BOOLEAN NOT NULL)";
                createTable = new SqliteCommand(tableCommand, db);
                try
                {
                    createTable.ExecuteReader(); //Execute command, throws SqliteException error if command doesn't execute properly
                }
                catch (SqliteException e)
                {
                    Console.WriteLine("Table error: " + e);
                }
                tableCommand = "CREATE TABLE IF NOT EXISTS Log (LogEntryId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Emp_id INTEGER CONSTRAINT Emp_idConstraint REFERENCES Login (Emp_id) ON DELETE NO ACTION ON UPDATE CASCADE NOT NULL, LotNum VARCHAR REFERENCES Sample (LotNum) ON DELETE NO ACTION ON UPDATE CASCADE NOT NULL, WhenModifed DATETIME NOT NULL, Patient_id VARCHAR (12), Rep_id VARCHAR (12), LogType CHAR (1) NOT NULL)";
                createTable = new SqliteCommand(tableCommand, db);
                try
                {
                    createTable.ExecuteReader(); //Execute command, throws SqliteException error if command doesn't execute properly
                }
                catch (SqliteException e)
                {
                    Console.WriteLine("Table error: " + e);
                }
                db.Close();
            }
        }


        // Method to grab entries from the SQLite database
        public static List<string> Grab_Entries(string search)
        {
            List<string> entries = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT NameandDosage from Sample", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Console.WriteLine(error);
                    db.Close();
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
        // Method to insert Samples into the Sample table
        // Needs to check if Sample exists

        public static bool Check_Sample(object sender, RoutedEventArgs e, string LotNumber)
        {
            bool check = false;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT LotNumber from Sample", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Console.WriteLine("Exception:" + error);
                    db.Close();
                    return false;
                }
                while (query.Read())
                {
                    var tmp = query.GetString(0);
                    if (tmp.Contains(LotNumber))
                    {
                        check = true;
                    }
                }
                db.Close();
            }
                return check;
        }
        public static bool Update_Sample(object sender, RoutedEventArgs e, string LotNumber, string count)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "UPDATE Sample SET Count = @Entry2 + Count WHERE LotNumber = @Entry1;"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", LotNumber);
                insertCommand.Parameters.AddWithValue("@Entry2", count);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Console.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }
        public static bool Add_Sample(object sender, RoutedEventArgs e, string LotNumber, string NameandDosage, string count, string expirationdate, string isExpired)
        {
            bool check = true;
            if(Check_Sample(sender, e, LotNumber))
            {
                Update_Sample(sender, e, LotNumber, count);
                return true;
            }
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO Sample VALUES (@Entry1, @Entry2, @Entry3, @Entry 4, @Entry5);"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", LotNumber);
                insertCommand.Parameters.AddWithValue("@Entry2", NameandDosage);
                insertCommand.Parameters.AddWithValue("@Entry3", count);
                insertCommand.Parameters.AddWithValue("@Entry4", expirationdate);
                insertCommand.Parameters.AddWithValue("@Entry5", isExpired);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Console.WriteLine("Exception: " + error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }
        // Method to insert new Employees into the Employee table

        public static bool Add_Employee(object sender, RoutedEventArgs e, string empID, string pin, string isActive)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO Login VALUES (@Entry1, @Entry2, @Entry3);"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", empID);
                insertCommand.Parameters.AddWithValue("@Entry2", pin);
                insertCommand.Parameters.AddWithValue("@Entry3", isActive);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Console.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }
        // Method to insert log line into Log table

        public static bool Add_Log(object sender, RoutedEventArgs e, string empID, string LotNumber, string patientID, string RepID, string LogType)
        {
            bool check = true;

            return check;
        }
        // Method to insert text into the SQLite database
        public static void Add_Text(object sender, RoutedEventArgs e, string inputVal)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO Sample VALUES (NULL, @Entry);"
                };
                insertCommand.Parameters.AddWithValue("@Entry", inputVal);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Console.WriteLine(error);
                    return;
                }
                db.Close();
            }
        }

        // Method to grab Text_Entry column from MyTable table in SQLite database
        public static List<String> Grab_Entries_col()
        {
            List<string> entries = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT NameandDosage from Sample", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Console.WriteLine("Error: " + error);
                    db.Close();
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
