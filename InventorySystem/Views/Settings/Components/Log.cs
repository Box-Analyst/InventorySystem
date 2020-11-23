using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Views.Settings.Components
{
    class Log
    {
        public int empID { get; set; }
        public string LotNum { get; set; }
        public DateTime LastModified { get; set; }
        public string PatientID { get; set; }
        public string RepID { get; set; }
        public string LogType { get; set; }

    }
}