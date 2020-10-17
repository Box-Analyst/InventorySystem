using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace InventorySystem.Views.Settings
{
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();

            object Current_Theme = Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"];

            if (Current_Theme == null)
            {
                Theme_Picker.PlaceholderText = "System (default)";
            }
            else if ((int)Current_Theme == 0)
            {
                Theme_Picker.PlaceholderText = "Light";
            }
            else if ((int)Current_Theme == 1)
            {
                Theme_Picker.PlaceholderText = "Dark";
            }
        }

        async private void Theme_Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Theme_Picker.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Light")
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"] = 0;
            }
            else if (Theme_Picker.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Dark")
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"] = 1;
            }
            else
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"] = null;
            }
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync("test");
            //Frame rootFrame = Window.Current.Content as Frame;
            //rootFrame.Navigate(typeof(MainPage));
            //Frame.Navigate(typeof(Settings));
        }

        private void Theme_Picker_DropDownClosed(object sender, object e)
        {

        }
    }
}
