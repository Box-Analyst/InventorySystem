using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using System.IO;

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
            else { this.Frame.Navigate(typeof(SettingsView), GetEmpID()); }
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

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var LocalState = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            string activeDBString = LocalState + @"\SamplesDB.db";
            string bakDB = LocalState + @"\SamplesDB." + DateTime.Now.Ticks + ".bak";
            File.Copy(activeDBString, bakDB, true);

            if (File.Exists(bakDB))
            {
                var activeDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("SamplesDB.db");

                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
                picker.FileTypeFilter.Add(".db");

                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
                    Windows.Storage.CachedFileManager.DeferUpdates(activeDB);
                    await Windows.Storage.FileIO.WriteBufferAsync(activeDB, buffer);

                    Windows.Storage.Provider.FileUpdateStatus status =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(activeDB);

                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        ExportSuccess.Text = "File " + file.Name + " was imported.";
                    }
                    else
                    {
                        ExportSuccess.Text = "File " + file.Name + " couldn't be imported.";
                    }
                }
                else
                {
                    ExportSuccess.Text = "Operation cancelled.";
                }
            }
            else
            {
                ExportSuccess.Text = "Failed to backup existing DB before import.";
            }
            ExportSuccess.Visibility = Visibility.Visible;
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var activeDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("SamplesDB.db");
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(activeDB);

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("SQLite Database", new List<string>() { ".db" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "SamplesDB";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                //await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                await Windows.Storage.FileIO.WriteBufferAsync(file, buffer);

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    ExportSuccess.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    ExportSuccess.Text = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                ExportSuccess.Text = "Operation cancelled.";
            }
            ExportSuccess.Visibility = Visibility.Visible;
        }
    }
}
