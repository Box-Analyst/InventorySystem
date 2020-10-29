using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using System;
using System.Diagnostics;
using InventorySystem.Views.Shell;

namespace InventorySystem.Views.Admin
{
    public sealed partial class AdminView : Page
    {
        public AdminView()
        {
            this.InitializeComponent();
            Output.ItemsSource = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", null);
        }

        private void Add_Text(object sender, RoutedEventArgs e)
        {
            SQL.ManageDB.Add_Text(sender, e, InputBox.Text);
            Output.ItemsSource = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", null);
        }

        private async void Import_Database(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".db");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                string PickedFile = file.Path;

                SQL.ManageDB.ExportDB(PickedFile, null, "import");

            }
            else
            {
                Debug.WriteLine("No file was selected.");
            }
        }

        private async void Export_Database(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                string PickedFolder = folder.Path;

                SQL.ManageDB.ExportDB(null, PickedFolder + @"\SamplesDB.db", "export");
            }
            else
            {
                Debug.WriteLine("No folder was selected.");
            }
        }
    }
}
