﻿using Windows.UI.Xaml;

namespace InventorySystem.Views.Shell
{
    class Theme
    {
        public static void InitializeTheme()
        {
            // Get system theme.
            var DefaultTheme = new Windows.UI.ViewManagement.UISettings();
            var uiTheme = DefaultTheme.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background).ToString();

            // Set app settings accordingly.
            if (uiTheme == "#FF000000")
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["systemThemeSetting"] = 1;
            }
            else if (uiTheme == "#FFFFFFFF")
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["systemThemeSetting"] = 0;
            }

            // Check if user has set a theme pref in the app
            object userThemeValue = Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"];
            if (userThemeValue != null)
            {
                // If so, apply theme choice.
                App.Current.RequestedTheme = (ApplicationTheme)(int)userThemeValue;
            }
            else
            {
                // Otherwise, use system theme.
                object sysThemeValue = Windows.Storage.ApplicationData.Current.LocalSettings.Values["systemThemeSetting"];
                App.Current.RequestedTheme = (ApplicationTheme)(int)sysThemeValue;
            }
        }
    }
}