#region copyright

// Copyright (c) Box Analyst. All rights reserved.
// This code is licensed under the GNU AGPLv3 License.

#endregion copyright

using InventorySystem.Views.Login;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using Windows.UI.Xaml;

namespace InventorySystem.SQL
{
    internal static class ManageDB
    {
        // Method to initialize database
        public static void InitializeDB()
        {
            //SqliteEngine.UseWinSqlite3(); //Configuring library to use SDK version of SQLite
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db")) //Name of .db file doesn't matter, but should be consistent across all SqliteConnection objects
            {
                db.Open(); //Open connection to database

                const string tableCommand1 = "CREATE TABLE IF NOT EXISTS Login (Emp_id NUMERIC PRIMARY KEY NOT NULL UNIQUE, Salt VARCHAR (64), Pin VARCHAR (64) NOT NULL, IsActive BOOLEAN NOT NULL, lastLoggedIn DATETIME, PrivLevel NUMERIC)";
                SqliteCommand createTable = new SqliteCommand(tableCommand1, db);

                try
                {
                    createTable.ExecuteReader(); //Execute command, throws SqliteException error if command doesn't execute properly
                }
                catch (SqliteException e)
                {
                    Debug.WriteLine("Table error: " + e);
                }
                AddAdminAccount();
                const string tableCommand2 = "CREATE TABLE IF NOT EXISTS Sample (LotNum VARCHAR PRIMARY KEY NOT NULL UNIQUE, NameandDosage VARCHAR(50) NOT NULL, Count INTEGER NOT NULL, ExpirationDate VARCHAR NOT NULL, isExpired BOOLEAN NOT NULL)";
                createTable = new SqliteCommand(tableCommand2, db);
                try
                {
                    createTable.ExecuteReader(); //Execute command, throws SqliteException error if command doesn't execute properly
                }
                catch (SqliteException e)
                {
                    Debug.WriteLine("Table error: " + e);
                }
                const string tableCommand3 = "CREATE TABLE IF NOT EXISTS Log (LogEntryId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Emp_id INTEGER NOT NULL, LotNum VARCHAR NOT NULL, WhenModifed DATETIME NOT NULL, Patient_id VARCHAR (12), Rep_id VARCHAR (12), LogType CHAR (1) NOT NULL)";
                createTable = new SqliteCommand(tableCommand3, db);
                try
                {
                    createTable.ExecuteReader(); //Execute command, throws SqliteException error if command doesn't execute properly
                }
                catch (SqliteException e)
                {
                    Debug.WriteLine("Table error: " + e);
                }
                db.Close();
            }

            if (Views.Settings.Components.Settings.FetchSetting("firstRun")?.ToString() != "1")
            {
                PopulateTestData(1000);
                Views.Settings.Components.Settings.ModifySetting("firstRun", "1");
            }
        }

        private static void AddAdminAccount()
        {
            int empID = 1;
            PasswordHash hash = new PasswordHash("password");
            hash.SetHash();
            string hashedPW = hash.GetHash();
            string salt = hash.GetSalt();
            int privLevel = 0;

            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("Select * from Login", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine("Error: " + error);
                    db.Close();
                    return;
                }
                if (query.HasRows == false)
                {
                    SqliteCommand insertCommand = new SqliteCommand
                    {
                        Connection = db,

                        //Use parameterized query to prevent SQL injection attacks
                        CommandText = "Insert into Login Values(@employeeID, @salt, @pw, @isActive, @lastLoggedIn, @privLevel);"
                    };
                    insertCommand.Parameters.AddWithValue("@employeeID", empID);
                    insertCommand.Parameters.AddWithValue("@salt", salt);
                    insertCommand.Parameters.AddWithValue("@pw", hashedPW);
                    insertCommand.Parameters.AddWithValue("@isActive", true);
                    insertCommand.Parameters.AddWithValue("@lastLoggedIn", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@privLevel", privLevel);
                    try
                    {
                        insertCommand.ExecuteReader();
                    }
                    catch (SqliteException error)
                    {
                        Debug.WriteLine("Exception: " + error);
                    }
                }
                db.Close();
            }
        }

        //Sets accounts inactive if they aren't used for 90 days
        public static void SetAcctsExpired()
        {
            List<string> expAccts = Grab_Entries("Login", "lastLoggedIn", "lastLoggedIn", null);
            var cultureInfo = new CultureInfo("en-US");
            DateTime localDate = DateTime.Now;
            foreach (string s in expAccts)
            {
                DateTime expDate = DateTime.Parse(s, cultureInfo);
                var isActive = true;
                if (localDate >= expDate.AddDays(90))
                {
                    isActive = false;
                    using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
                    {
                        db.Open();
                        SqliteCommand updateCommand = new SqliteCommand
                        {
                            Connection = db,
                            CommandText = "UPDATE Login SET isActive = @isActive WHERE lastLoggedIn = @lastLoggedIn;"
                        };
                        updateCommand.Parameters.AddWithValue("@isActive", isActive);
                        updateCommand.Parameters.AddWithValue("@lastLoggedIn", expDate);
                        try
                        {
                            updateCommand.ExecuteReader();
                        }
                        catch (SqliteException error)
                        {
                            Debug.WriteLine(error);
                        }
                        db.Close();
                    }
                }
                else { isActive = true; }
            }
        }

        public static void UpdateAcctLoggedIn(int empID)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand updateCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "UPDATE Login SET lastLoggedIn = @lastLoggedIn WHERE Emp_id = @empID;"
                };
                updateCommand.Parameters.AddWithValue("@lastLoggedIn", DateTime.Now);
                updateCommand.Parameters.AddWithValue("@empID", empID);
                try
                {
                    updateCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine(error);
                }
                db.Close();
            }
        }

        // Method to grab entries from the SQLite database
        public static List<string> Grab_Entries(string table, string returnColumn, string comparisonColumn, object search)
        {
            List<string> entries = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "SELECT " + returnColumn + ", " + comparisonColumn + " FROM " + table
                };
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine("Error: " + error);
                    db.Close();
                    return entries;
                }
                while (query.Read())
                {
                    // if search is specified, only return values containing that query
                    // otherwise return entire column
                    if (search != null && query.GetString(1).ToLower().Contains(search.ToString().ToLower()))
                    {
                        entries.Add(query.GetString(0));
                    }
                    else if (search == null)
                    {
                        entries.Add(query.GetString(0));
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
                SqliteCommand selectCommand = new SqliteCommand("SELECT LotNum FROM Sample", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine("Exception:" + error);
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

        public static bool Update_Sample(object sender, RoutedEventArgs e, string LotNumber, int count)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "UPDATE Sample SET Count = @Entry2 + Count WHERE LotNum = @Entry1;"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", LotNumber);
                insertCommand.Parameters.AddWithValue("@Entry2", count);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        public static bool Add_Sample(object sender, RoutedEventArgs e, string LotNumber, string NameandDosage, int count, string expirationdate, bool isExpired)
        {
            bool check = true;
            if (Check_Sample(sender, e, LotNumber))
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
                    CommandText = "INSERT INTO Sample VALUES (@Entry1, @Entry2, @Entry3, @Entry4, @Entry5);"
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
                    Debug.WriteLine("Exception: " + error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        public static bool Delete_Sample(object sender, RoutedEventArgs e, string LotNumber)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand deleteCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "DELETE FROM Sample WHERE LotNum = @Entry1;"
                };
                deleteCommand.Parameters.AddWithValue("@Entry1", LotNumber);
                try
                {
                    deleteCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine("Exception: " + error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        public static bool Delete_Expired(object sender, RoutedEventArgs e)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand deleteCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "DELETE FROM Sample WHERE isExpired = 1;"
                };
                try
                {
                    deleteCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine("Exception: " + error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        public static bool Check_ExpirationDate_RegEx(string expirationdate)
        {
            bool check = false;
            string regexString =
            @"^(0[1-9]|1[012])[/](0[1-9]|[12][0-9]|3[01])[/](19|20|21)\d\d$";
            RegexStringValidator myRegexValidator =
             new RegexStringValidator(regexString);

            // Determine if the object to validate can be validated.
            Debug.WriteLine("CanValidate: {0}",
              myRegexValidator.CanValidate(expirationdate.GetType()));

            try
            {
                // Attempt validation.
                myRegexValidator.Validate(expirationdate);
                check = true;
            }
            catch (ArgumentException error)
            {
                // Validation failed.
                Debug.WriteLine("Error: {0}", error.Message);
                check = false;
            }
            return check;
        }

        public static bool Check_LotNumber_RegEx(string lotNum)
        {
            bool check = false;
            string regexString =
            @"^[a-zA-Z0-9]+$";
            RegexStringValidator myRegexValidator =
             new RegexStringValidator(regexString);

            // Determine if the object to validate can be validated.
            Debug.WriteLine("CanValidate: {0}",
              myRegexValidator.CanValidate(lotNum.GetType()));

            try
            {
                // Attempt validation.
                myRegexValidator.Validate(lotNum);
                check = true;
            }
            catch (ArgumentException error)
            {
                // Validation failed.
                Debug.WriteLine("Error: {0}", error.Message);
                check = false;
            }
            return check;
        }

        public static bool Check_NameAndDosage_RegEx(string nameAndDosage)
        {
            bool check = false;
            string regexString =
            @"^[a-zA-Z]+ [0-9]+[a-zA-Z]+$";
            RegexStringValidator myRegexValidator =
             new RegexStringValidator(regexString);

            // Determine if the object to validate can be validated.
            Debug.WriteLine("CanValidate: {0}",
              myRegexValidator.CanValidate(nameAndDosage.GetType()));

            try
            {
                // Attempt validation.
                myRegexValidator.Validate(nameAndDosage);
                check = true;
            }
            catch (ArgumentException error)
            {
                // Validation failed.
                Debug.WriteLine("Error: {0}", error.Message);
                check = false;
            }
            return check;
        }

        public static bool Check_Count_RegEx(string count)
        {
            bool check = false;
            string regexString =
            @"^[0-9]+$";
            RegexStringValidator myRegexValidator =
             new RegexStringValidator(regexString);

            // Determine if the object to validate can be validated.
            Debug.WriteLine("CanValidate: {0}",
              myRegexValidator.CanValidate(count.GetType()));

            try
            {
                // Attempt validation.
                myRegexValidator.Validate(count);
                check = true;
            }
            catch (ArgumentException error)
            {
                // Validation failed.
                Debug.WriteLine("Error: {0}", error.Message);
                check = false;
            }
            return check;
        }

        public static bool Check_RepID_RegEx(string repID)
        {
            bool check = false;
            string regexString =
            @"^[a-zA-Z0-9]+$";
            RegexStringValidator myRegexValidator =
             new RegexStringValidator(regexString);

            // Determine if the object to validate can be validated.
            Debug.WriteLine("CanValidate: {0}",
              myRegexValidator.CanValidate(repID.GetType()));

            try
            {
                // Attempt validation.
                myRegexValidator.Validate(repID);
                check = true;
            }
            catch (ArgumentException error)
            {
                // Validation failed.
                Debug.WriteLine("Error: {0}", error.Message);
                check = false;
            }
            return check;
        }

        // Method to check if a sample is expired
        public static bool Check_IsExpired(string expirationdate)
        {
            bool check = false;
            var cultureInfo = new CultureInfo("en-US");
            DateTime localDate = DateTime.Now;
            DateTime expirationDate = DateTime.Parse(expirationdate, cultureInfo);
            if (localDate >= expirationDate)
            {
                check = true;
            }
            return check;
        }

        // Method to update isExpired
        public static void Update_IsExpired()
        {
            List<string> entries = new List<string>();
            List<string> entryLotNumbers = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT ExpirationDate, LotNum FROM Sample WHERE isExpired = 0", db);
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
                    entryLotNumbers.Add(tmp1);
                }
                for (int i = 0; i < entries.Count; i++)
                {
                    if (!Check_IsExpired(entries[i])) continue;
                    SqliteCommand insertCommand = new SqliteCommand
                    {
                        Connection = db,

                        //Use parameterized query to prevent SQL injection attacks
                        CommandText = "UPDATE Sample SET isExpired = @Entry2 WHERE LotNum = @Entry1;"
                    };
                    insertCommand.Parameters.AddWithValue("@Entry1", entryLotNumbers[i]);
                    insertCommand.Parameters.AddWithValue("@Entry2", true);
                    try
                    {
                        insertCommand.ExecuteReader();
                    }
                    catch (SqliteException error)
                    {
                        Debug.WriteLine(error);
                        db.Close();
                        return;
                    }
                }
                db.Close();
            }
        }

        // Method to insert new Employees into the Employee table
        public static bool Add_Employee(object sender, RoutedEventArgs e, int empID, string salt, string pin, bool isActive, DateTime lastLoggedIn, int privLevel)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO Login VALUES (@Entry1, @Entry2, @Entry3, @Entry4, @Entry5, @Entry6);"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", empID);
                insertCommand.Parameters.AddWithValue("@Entry2", salt);
                insertCommand.Parameters.AddWithValue("@Entry3", pin);
                insertCommand.Parameters.AddWithValue("@Entry4", isActive);
                insertCommand.Parameters.AddWithValue("@Entry5", lastLoggedIn);
                insertCommand.Parameters.AddWithValue("@Entry6", privLevel);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        public static bool Update_Employee(object sender, RoutedEventArgs e, int empID, string salt, string pin, bool isActive, DateTime lastLoggedIn)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "UPDATE Login SET salt = @Entry2, pin = @Entry3, isActive = @Entry4, lastLoggedIn = @Entry5 WHERE Emp_id = @Entry1;"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", empID);
                insertCommand.Parameters.AddWithValue("@Entry2", salt);
                insertCommand.Parameters.AddWithValue("@Entry3", pin);
                insertCommand.Parameters.AddWithValue("@Entry4", isActive);
                insertCommand.Parameters.AddWithValue("@Entry5", lastLoggedIn);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        public static bool Update_Employee(object sender, RoutedEventArgs e, int empID, string salt, string pin, bool isActive, DateTime lastLoggedIn, int privLevel)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "UPDATE Login SET salt = @Entry2, pin = @Entry3, isActive = @Entry4, lastLoggedIn = @Entry5, PrivLevel = @Entry6 WHERE Emp_id = @Entry1;"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", empID);
                insertCommand.Parameters.AddWithValue("@Entry2", salt);
                insertCommand.Parameters.AddWithValue("@Entry3", pin);
                insertCommand.Parameters.AddWithValue("@Entry4", isActive);
                insertCommand.Parameters.AddWithValue("@Entry5", lastLoggedIn);
                insertCommand.Parameters.AddWithValue("@Entry6", privLevel);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        public static bool Update_Employee(object sender, RoutedEventArgs e, int empID, int privLevel)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "UPDATE Login SET PrivLevel = @Entry2 WHERE Emp_id = @Entry1;"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", empID);
                insertCommand.Parameters.AddWithValue("@Entry2", privLevel);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
            return check;
        }

        // Method to insert log line into Log table
        // This method should be used cautiously!
        public static bool Add_Log(object sender, RoutedEventArgs e, string empID, string LotNumber, string whenModified, string patientID, string RepID, string LogType)
        {
            bool check = true;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    //Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO Log VALUES (NULL, @Entry1, @Entry2, @Entry3, @Entry4, @Entry5, @Entry6);"
                };
                insertCommand.Parameters.AddWithValue("@Entry1", empID);
                insertCommand.Parameters.AddWithValue("@Entry2", LotNumber);
                insertCommand.Parameters.AddWithValue("@Entry3", whenModified);
                insertCommand.Parameters.AddWithValue("@Entry4", patientID);
                insertCommand.Parameters.AddWithValue("@Entry5", RepID);
                insertCommand.Parameters.AddWithValue("@Entry6", LogType);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    Debug.WriteLine(error);
                    check = false;
                }
                db.Close();
            }
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
                    Debug.WriteLine(error);
                    return;
                }
                db.Close();
            }
        }

        public static bool CheckEmployee(int employeeID)
        {
            bool check;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT * from Login WHERE Emp_id = @empID", db);
                selectCommand.Parameters.AddWithValue("@empID", employeeID);
                SqliteDataReader query = selectCommand.ExecuteReader();
                check = query.HasRows;
                db.Close();
            }
            return check;
        }

        public static bool CheckPassword(string password, int employeeID)
        {
            bool check;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("Select Pin from Login Where Emp_id = @empID and Pin = @password", db);
                selectCommand.Parameters.AddWithValue("@empID", employeeID);
                selectCommand.Parameters.AddWithValue("@password", password);
                SqliteDataReader query = selectCommand.ExecuteReader();
                check = query.HasRows;
                db.Close();
            }
            return check;
        }

        public static bool CheckAcctActive(int employeeID)
        {
            bool check;
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT * from Login WHERE Emp_id = @empID and isActive = true", db);
                selectCommand.Parameters.AddWithValue("@empID", employeeID);
                SqliteDataReader query = selectCommand.ExecuteReader();
                check = query.HasRows;
                db.Close();
            }
            return check;
        }

        public static bool IsEmpty()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("Select COUNT(*) from Sample", db);
                SqliteDataReader query = selectCommand.ExecuteReader();
                bool check = true;
                if (query.Read())
                {
                    var numRows = int.Parse($"{query[0]}");
                    if (numRows > 0)
                    {
                        check = false;
                    }
                    return check;
                }
                db.Close();
                return check;
            }
        }

        // Method to check if a sample is about to expire
        public static bool Check_ExpiresSoon(string expirationdate, double noticeTime)
        {
            bool check = false;
            var cultureInfo = new CultureInfo("en-US");
            DateTime localDate = DateTime.Now;
            DateTime expirationDate = DateTime.Parse(expirationdate, cultureInfo);
            if (localDate >= expirationDate.AddDays(noticeTime))
            {
                check = true;
            }
            return check;
        }

        // Temp method for testing purposes
        private static void PopulateTestData(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var random = new Random();
                var boolList = new List<int> { 0, 1 };
                int index = random.Next(boolList.Count);

                DateTime start = new DateTime(2020, 1, 1);
                DateTime end = new DateTime(2021, 12, 31);
                int range = (end - start).Days;
                var randDate = start.AddDays(random.Next(range));

                var LotNumber = i.ToString();
                var NameandDosage = "Test " + i + "mg";
                var count = random.Next(1, int.MaxValue);
                var expirationdate = randDate;
                var isExpired = boolList[index];

                Debug.WriteLine(i);

                using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
                {
                    db.Open();
                    SqliteCommand insertCommand = new SqliteCommand
                    {
                        Connection = db,

                        //Use parameterized query to prevent SQL injection attacks
                        CommandText = "INSERT INTO Sample VALUES (@Entry1, @Entry2, @Entry3, @Entry4, @Entry5);"
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
                        Debug.WriteLine("Exception: " + error);
                    }
                    db.Close();
                }
            }
        }
    }
}