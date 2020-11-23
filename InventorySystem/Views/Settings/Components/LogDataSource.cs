using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Views.Settings.Components
{
    class LogDataSource
    {
        private static List<Log> logList = new List<Log>();

        public static List<Log> GetLogs()
        {
            using (SqliteConnection db = new SqliteConnection("Filename=SamplesDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "SELECT Emp_id, LotNum, WhenModified, Patient_id, Rep_id, LogType FROM Log WHERE LotNum"
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
                    return logList;
                }
                while (query.Read())
                {
                    Log log = new Log()
                    {
                        empID = int.Parse(query.GetString(0)),
                        LotNum = query.GetString(1),
                        LastModified = DateTime.Parse(query.GetString(2)),
                        PatientID = query.GetString(3),
                        RepID = query.GetString(4),
                        LogType = query.GetString(5)
                    };
                    logList.Add(log);
                }
                db.Close();
            }
            return logList;
        }
    }
}