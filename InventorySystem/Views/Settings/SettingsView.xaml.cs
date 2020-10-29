using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

namespace InventorySystem.Views.Settings
{
    public sealed partial class SettingsView : Page
    {
        private string empID;
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

        //When Settings is navigated to, empID is passed to this function and stored in private class variable empID
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter.ToString();
        }
        private async void ThemePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContentDialog areYouSure = new ContentDialog
            {
                Title = "Are You Sure?",
                Content = "The application will restart upon changing this setting. Select Yes to continue, Cancel to go back.",
                PrimaryButtonText = "Cancel",
                CloseButtonText = "Yes"

            };

            ContentDialogResult result = await areYouSure.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {

                switch (ThemePicker.SelectedItem?.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
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
                AppRestartFailureReason result2 = await CoreApplication.RequestRestartAsync("test");
            }
            else {this.Frame.Navigate(typeof(SettingsView), GetEmpID());}
        }

        private void ThemePicker_DropDownClosed(object sender, object e)
        {

        }

        private async void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdmin() == true)
            {
                this.Frame.Navigate(typeof(Login.Components.AddUsers), GetEmpID());
            }
            else
            {
                ContentDialog noPrivilege = new ContentDialog
                {
                    Title = "Insufficient Privileges",
                    Content = "You must be signed in to the Admin account to add users.",
                    CloseButtonText = "Ok"
                };
                ContentDialogResult result = await noPrivilege.ShowAsync();
            }
        }

        //Checks if the empID is the designated admin account number
        private bool IsAdmin()
        {
            bool isAdmin = false;
            if (empID == "1")
            {
                isAdmin = true;
            }

            return isAdmin;
        }

        public string GetEmpID()
        {
            return empID;
        }
    }
}
