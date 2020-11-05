using System;
using System.Collections;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace InventorySystem.Views.Shell
{
    public sealed partial class MainNavView
    {
        private string empID;
        public MainNavView()
        {
            this.InitializeComponent();
        }

        //When MainNavView is navigated to, empID is passed to this function and stored in private class variable empID
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter?.ToString();
        }

        // NavView stuff
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // you can also add items in code behind
            //NavView.MenuItems.Add(new NavigationViewItemSeparator());
            //NavView.MenuItems.Add(new NavigationViewItem()
            //{ Content = "My content", Icon = new SymbolIcon(Symbol.Folder), Tag = "content" });

            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "home")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            // Load Home on app start
            ContentFrame.Navigate(typeof(Home.HomeView), empID);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem == null) NavView.SelectedItem = null;
            else if (args.IsSettingsInvoked) ContentFrame.Navigate(typeof(Settings.SettingsView), GetEmpID());
            else NavView_Navigate(sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem));
        }

        // Navbar items
        private void NavView_Navigate(NavigationViewItem item)
        {
            //Each case will pass the employeeID to the page to be navigated to for logging purposes
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(Home.HomeView), empID);
                    break;

                case "alerts":
                    ContentFrame.Navigate(typeof(Notifications.NotifyView), empID);
                    break;

                case "samples":
                    ContentFrame.Navigate(typeof(Samples.SamplesView), empID);
                    break;
            }
        }

        // AutoSuggestBox
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", "NameandDosage", sender.Text);
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {
                // Use args.QueryText to determine what to do.
            }
        }
        // End AutoSuggestBox

        // Add Sample button
        private void AppBarButton_Clicked(object sender, TappedRoutedEventArgs e)
        {
            NavView.SelectedItem = null;
            ContentFrame.Navigate(typeof(Samples.Components.AddSample), empID);
        }

        private async void SignOutButton_Clicked(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog areYouSure = new ContentDialog
            {
                Title = "Sign Out?",
                Content = "This will sign you out of the app and return to the login screen.",
                PrimaryButtonText = "Cancel",
                CloseButtonText = "Yes"

            };

            ContentDialogResult result = await areYouSure.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                NavView.SelectedItem = null;
                Frame.Navigate(typeof(LoginWindow));
            }
        }
    }
}
