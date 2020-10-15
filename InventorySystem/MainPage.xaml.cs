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
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Microsoft.QueryStringDotNET; // QueryString.NET
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;

// Import Microsoft.Data.Sqlite namespaces
using Microsoft.Data.Sqlite;

namespace InventorySystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // Track open app windows in a Dictionary.
        public static Dictionary<UIContext, AppWindow> AppWindows { get; set; }
            = new Dictionary<UIContext, AppWindow>();

        // Track the last opened dialog so you can close it if another dialog tries to open.
        public static ContentDialog CurrentDialog { get; set; } = null;

        public MainPage()
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
            ContentFrame.Navigate(typeof(Home));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Settings page
                ContentFrame.Navigate(typeof(Settings));
            }
            else
            {
                // find NavigationViewItem with Content that equals InvokedItem
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(Home));
                    break;

                case "apps":
                    ContentFrame.Navigate(typeof(Home));
                    break;

                case "games":
                    ContentFrame.Navigate(typeof(Home));
                    break;

                case "music":
                    ContentFrame.Navigate(typeof(Home));
                    break;

                case "admin":
                    ContentFrame.Navigate(typeof(Console));
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
                sender.ItemsSource = Grab_Entries(sender.Text);
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

        // SQL stuff starts here
        // Method to insert text into the SQLite database
        private void Add_Text(object sender, RoutedEventArgs e)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                //Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", sender);
                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    //Handle error
                    return;
                }
                db.Close();
            }
            ContentFrame.Navigate(ContentFrame.Content.GetType());
        }

        // Method to grab Text_Entry column from MyTable table in SQLite database
        // and return values containing search
        private List<String> Grab_Entries(string search)
        {
            List<String> entries = new List<string>();
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand("SELECT Text_Entry from MyTable", db);
                SqliteDataReader query;
                try
                {
                    query = selectCommand.ExecuteReader();
                }
                catch (SqliteException error)
                {
                    //Handle error
                    return entries;
                }
                while (query.Read())
                {
                    var tmp = query.GetString(0);
                    if (tmp.Contains(search))
                    {
                        entries.Add(tmp);
                    }
                }
                db.Close();
            }
            return entries;
        }
        // End AutoSuggestBox


        // Add Sample button
        private async void AppBarButton_Clicked(object sender, RoutedEventArgs e)
        {
            // Create a new window.
            AppWindow addSampleViewWindow = await AppWindow.TryCreateAsync();

            // Create a Frame and navigate to the Page you want to show in the new window.
            Frame appWindowContentFrame = new Frame();
            appWindowContentFrame.Navigate(typeof(AddSample));

            // Get a reference to the page instance and assign the
            // newly created AppWindow to the MyAppWindow property.
            AddSample page = (AddSample)appWindowContentFrame.Content;
            page.MyAppWindow = addSampleViewWindow;

            // Attach the XAML content to the window.
            ElementCompositionPreview.SetAppWindowContent(addSampleViewWindow, appWindowContentFrame);

            // Add the new page to the Dictionary using the UIContext as the Key.
            AppWindows.Add(appWindowContentFrame.UIContext, addSampleViewWindow);
            addSampleViewWindow.Title = "App Window " + AppWindows.Count.ToString();

            // When the window is closed, be sure to release XAML resources
            // and the reference to the window.
            addSampleViewWindow.Closed += delegate
            {
                MainPage.AppWindows.Remove(appWindowContentFrame.UIContext);
                appWindowContentFrame.Content = null;
                addSampleViewWindow = null;
            };

            // Show the window.
            await addSampleViewWindow.TryShowAsync();
        }

        // Notify icon click
        // Unused code for now, need to move to wherever we're gonna handle notifications
        private void NotifyBarButton_Click(object sender, RoutedEventArgs e)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
            {
                new AdaptiveText()
                {
                    Text = "Hello World"
                },
                new AdaptiveText()
                {
                    Text = "This is a simple toast message"
                }
            }
                    }
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
    }
}
