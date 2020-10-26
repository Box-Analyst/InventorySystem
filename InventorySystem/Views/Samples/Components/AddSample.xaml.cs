using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;

namespace InventorySystem.Views.Samples.Components
{
    public sealed partial class AddSample : Page
    {
        public AddSample()
        {
            this.InitializeComponent();
        }

        private void Add_Sample(object sender, RoutedEventArgs e)
        {
            bool isExpired = false;
            if (SQL.ManageDB.Check_ExpirationDate_RegEx(ExpirationDateBox.Text))
            {
                if(SQL.ManageDB.Check_IsExpired(ExpirationDateBox.Text))
                {
                    isExpired = true;
                }
                SQL.ManageDB.Add_Sample(sender, e, LotNumBox.Text, NameAndDosageBox.Text, Int32.Parse(CountBox.Text), ExpirationDateBox.Text, isExpired);
            } else
            {
                DisplayExpirationDateError();
            }
        }

        private async void DisplayExpirationDateError()
        {
            ContentDialog addExpirationDateError = new ContentDialog
            {
                Title = "Invalid Expiration Date Input",
                Content = "Expiration Date formatted Incorrectly. \nFormatting should be MM/DD/YYYY",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await addExpirationDateError.ShowAsync();
        }

    }
}
