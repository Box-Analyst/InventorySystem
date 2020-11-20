#region copyright

// Copyright (c) Box Analyst. All rights reserved.
// This code is licensed under the GNU AGPLv3 License.

#endregion copyright

namespace InventorySystem.Views.Settings.Components
{
    internal static class Settings
    {
        // Method for modifying app settings
        public static string ModifySetting(string settingName, string settingValue)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[settingName] = settingValue;
            return (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values[settingName];
        }

        // Method for fetching app settings
        public static object FetchSetting(object settingName)
        {
            if (settingName == null)
            {
                return null;
            }
            else
            {
                return (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values[(string)settingName];
            }
        }

        // Method for deleting app settings
        public static void RemoveSetting(string settingName)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(settingName);
        }
    }
}