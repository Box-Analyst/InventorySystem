using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InventorySystem.Views.Shell
{
    public sealed partial class MainNavView : Page
    {
        public MainNavView()
        {
            this.InitializeComponent();
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
            ContentFrame.Navigate(typeof(Home.HomeView));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Settings page
                ContentFrame.Navigate(typeof(Settings.SettingsView));
            }
            else
            {
                // find NavigationViewItem with Content that equals InvokedItem
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item);
            }
        }

        // Navbar items
        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(Home.HomeView));
                    break;

                case "alerts":
                    ContentFrame.Navigate(typeof(Notifications.NotifyView));
                    break;

                case "samples":
                    ContentFrame.Navigate(typeof(Samples.SamplesView));
                    break;

                case "admin":
                    ContentFrame.Navigate(typeof(Admin.AdminView));
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
                sender.ItemsSource = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", sender.Text);
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
        private void AppBarButton_Clicked(object sender, RoutedEventArgs e)
        {
            NewWindow.CreateNewWindow(typeof(Samples.Components.AddSample));
        }
    }
}
