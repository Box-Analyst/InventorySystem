using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Views.Settings.Components
{
    class Settings
    {
        // Method for modifying app settings
        public static string ModifySetting(string SettingName, string SettingValue)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[SettingName] = SettingValue;
            return (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values[SettingName];
        }

        // Method for fetching app settings
        public static object FetchSetting(object SettingName)
        {
            if (SettingName == null)
            {
                return null;
            }
            else
            {
                return (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values[(string)SettingName];
            }
        }

        // Method for deleting app settings
        public static void RemoveSetting(string SettingName)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(SettingName);
        }
    }
}
