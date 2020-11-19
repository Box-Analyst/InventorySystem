using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Automation.Peers;


namespace InventorySystem.Views.Samples.Components
{
    public class SampleDataSource
    {
        private static List<Sample> sampleList = new List<Sample>();

        //returns a list of all samples that aren't expired
        public static List<Sample> GetSamples()
        {
            var samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "isExpired", 0);
            string sampleNames = string.Join("', '", samples);
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "SELECT LotNum, NameandDosage, ExpirationDate, Count FROM Sample WHERE LotNum IN ('" + sampleNames + "')"
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
                    return sampleList;
                }
                while (query.Read())
                {
                        Sample sample = new Sample()
                        {
                            LotNum = query.GetString(0),
                            NameandDosage = query.GetString(1),
                            ExpirationDate = query.GetString(2),
                            Count = int.Parse(query.GetString(3))
                        };
                        sampleList.Add(sample);
                }
                db.Close();
            }
            return sampleList;
        }

    
        //returns a list of samples matching the string search from autosuggest on MainNav
        public static List<Sample> GetSearchedSample(string nameDose)
        {
                var samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "NameandDosage", nameDose);
                string sampleNames = string.Join("', '", samples);
                using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
                {
                    db.Open();
                    SqliteCommand selectCommand = new SqliteCommand
                    {
                        Connection = db,
                        CommandText = "SELECT LotNum, NameandDosage, ExpirationDate, Count FROM Sample WHERE LotNum IN ('" + sampleNames + "')"
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
                        return sampleList;
                    }
                    while (query.Read())
                    {
                        Sample sample = new Sample()
                        {
                            LotNum = query.GetString(0),
                            NameandDosage = query.GetString(1),
                            ExpirationDate = query.GetString(2),
                            Count = int.Parse(query.GetString(3))
                        };
                        sampleList.Add(sample);
                    }
                    db.Close();
                }
                return sampleList;
        }


    }
}
