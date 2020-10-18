using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

namespace InventorySystem.Views.Settings
{
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();

            object currentTheme = Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"];

            switch (currentTheme)
            {
                case null:
                    ThemePicker.PlaceholderText = "System (default)";
                    break;

                case 0:
                    ThemePicker.PlaceholderText = "Light";
                    break;

                case 1:
                    ThemePicker.PlaceholderText = "Dark";
                    break;
            }
        }

        private async void ThemePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ThemePicker.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "Light":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"] = 0;
                    break;

                case "Dark":
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"] = 1;
                    break;

                default:
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["userThemeSetting"] = null;
                    break;
            }
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync("test");
            //Frame rootFrame = Window.Current.Content as Frame;
            //rootFrame.Navigate(typeof(MainPage));
            //Frame.Navigate(typeof(Settings));
        }

        private void ThemePicker_DropDownClosed(object sender, object e)
        {

        }
    }
}
