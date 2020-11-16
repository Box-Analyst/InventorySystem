using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace InventorySystem.Views.Settings
{
    public sealed partial class SettingsView : Page
    {
        private string empID;
        List<string> passedVars = new List<string>();
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
            passedVars.Clear();
            empID = e.Parameter?.ToString();
            passedVars.Add(empID);
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
            else { this.Frame.Navigate(typeof(SettingsView), empID); }
        }

        private void ThemePicker_DropDownClosed(object sender, object e)
        {

        }

        private async void AddNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdmin() == true)
            {
                this.Frame.Navigate(typeof(Login.Components.AddUsers), empID);
            }
            else
            {
                ContentDialog noPrivilege = new ContentDialog
                {
                    Title = "Insufficient Privileges",
                    Content = "You must be signed in to the Admin account to add/modify users.",
                    CloseButtonText = "Ok"
                };
                await noPrivilege.ShowAsync();
            }
        }

        private async void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            passedVars.Add("renew");
            if (IsAdmin() == true)
            {
                passedVars.Add("adminChange");
                this.Frame.Navigate(typeof(Components.RenewAccount), passedVars);
            }
            else
            {
                passedVars.RemoveAt(1);
                ContentDialog noPrivilege = new ContentDialog
                {
                    Title = "Insufficient Privileges",
                    Content = "You must be signed in to the Admin account to add users.",
                    CloseButtonText = "Ok"
                };
                await noPrivilege.ShowAsync();
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            passedVars.Add("reset");
            if (IsAdmin() == true)
            {
                passedVars.Add("adminChange");
                this.Frame.Navigate(typeof(Components.RenewAccount), passedVars);
            }
            else
            {
                passedVars.Add("nonAdminChange");
                this.Frame.Navigate(typeof(Components.RenewAccount), passedVars);
            }
        }

        //Checks if the empID is the designated admin account number
        private bool IsAdmin()
        {
            bool isAdmin = empID == SQL.ManageDB.Grab_Entries("Login", "Emp_id", "PrivLevel", 0)[0];
            return isAdmin;
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdmin() == true)
            {
                ContentDialog importBakAlert = new ContentDialog
                {
                    Title = "Backup Before Importing",
                    Content = "You must backup the existing database before importing.\nPlease select where you'd like to backup to on the following screen.",
                    CloseButtonText = "Ok"
                };
                ContentDialogResult resultBak = await importBakAlert.ShowAsync();

                string bakDB = "SamplesDB." + DateTime.Now.Ticks + ".db";

                var activeDBBak = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("SamplesDB.db");
                var bufferDBBak = await Windows.Storage.FileIO.ReadBufferAsync(activeDBBak);

                var savePicker = new Windows.Storage.Pickers.FileSavePicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder
                };
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Backup File", new List<string>() { ".bak" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = bakDB;

                Windows.Storage.StorageFile fileBak = await savePicker.PickSaveFileAsync();
                if (fileBak != null)
                {
                    // Prevent updates to the remote version of the file until
                    // we finish making changes and call CompleteUpdatesAsync.
                    Windows.Storage.CachedFileManager.DeferUpdates(fileBak);
                    // write to file
                    //await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                    await Windows.Storage.FileIO.WriteBufferAsync(fileBak, bufferDBBak);

                    // Let Windows know that we're finished changing the file so
                    // the other app can update the remote version of the file.
                    // Completing updates may require Windows to ask for user input.
                    Windows.Storage.Provider.FileUpdateStatus statusDBBak =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(fileBak);
                    if (statusDBBak == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        ContentDialog importAlert = new ContentDialog
                        {
                            Title = "Backup Successful",
                            Content = "Backup to " + fileBak.Path + " was successful.\nSelect the file you would like to import on the following screen.",
                            CloseButtonText = "Ok"
                        };
                        await importAlert.ShowAsync();

                        var activeDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("SamplesDB.db");

                        var picker = new Windows.Storage.Pickers.FileOpenPicker
                        {
                            ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder
                        };
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
                            ExportSuccess.Text = "Operation cancelled or interrupted.";
                        }
                    }
                    else
                    {
                        ExportSuccess.Text = "Failed to backup existing DB before import.";
                    }
                }
                else
                {
                    ExportSuccess.Text = "Operation cancelled or interrupted.";
                }

                ExportSuccess.Visibility = Visibility.Visible;
            }
            else
            {
                ContentDialog noPrivilege = new ContentDialog
                {
                    Title = "Insufficient Privileges",
                    Content = "You must be signed in to the Admin account to manage the database.",
                    CloseButtonText = "Ok"
                };
                await noPrivilege.ShowAsync();
            }
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdmin() == true)
            {
                var activeDB = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("SamplesDB.db");
                var buffer = await Windows.Storage.FileIO.ReadBufferAsync(activeDB);

                var savePicker = new Windows.Storage.Pickers.FileSavePicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder
                };
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
                        ExportSuccess.Text = "File " + file.Name + " was exported.";
                    }
                    else
                    {
                        ExportSuccess.Text = "File " + file.Name + " couldn't be exported.";
                    }
                }
                else
                {
                    ExportSuccess.Text = "Operation cancelled or interrupted.";
                }
                ExportSuccess.Visibility = Visibility.Visible;
            }
            else
            {
                ContentDialog noPrivilege = new ContentDialog
                {
                    Title = "Insufficient Privileges",
                    Content = "You must be signed in to the Admin account to manage the database.",
                    CloseButtonText = "Ok"
                };
                await noPrivilege.ShowAsync();
            }
        }
    }
}
