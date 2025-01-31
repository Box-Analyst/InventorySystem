﻿#region copyright

// Copyright (c) Box Analyst. All rights reserved.
// This code is licensed under the GNU AGPLv3 License.

#endregion copyright

using InventorySystem.Views.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly List<string> passedVars = new List<string>();

        public MainNavView()
        {
            InitializeComponent();
        }

        //When MainNavView is navigated to, empID is passed to this function and stored in private class variable empID
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter?.ToString();
            App.Current.IsIdleChanged += onIsIdleChanged;
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
                if (item is NavigationViewItem && item.Tag.ToString() == "alerts")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            // Load Home on app start
            ContentFrame.Navigate(typeof(NotifyView), empID);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem == null) NavView.SelectedItem = null;
            else if (args.IsSettingsInvoked) ContentFrame.Navigate(typeof(Settings.SettingsView), empID);
            else NavView_Navigate(sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem));
        }

        // Navbar items
        private void NavView_Navigate(NavigationViewItem item)
        {
            //Each case will pass the employeeID to the page to be navigated to for logging purposes
            switch (item.Tag)
            {
                case "alerts":
                    ContentFrame.Navigate(typeof(NotifyView), empID);
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
            sender.Text = args.SelectedItem.ToString();
            passedVars.Clear();
            passedVars.Add(empID);
            passedVars.Add(sender.Text);
            //passedVars.Add("true");
            Debug.WriteLine(sender.Text);
            ContentFrame.Navigate(typeof(Samples.SamplesView), passedVars);
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                passedVars.Clear();
                passedVars.Add(empID);
                passedVars.Add(sender.Text);
                //passedVars.Add("true");
                Debug.WriteLine(sender.Text);
                ContentFrame.Navigate(typeof(Samples.SamplesView), passedVars);
            }
            else
            {
                // Use args.QueryText to determine what to do.
                passedVars.Clear();
                passedVars.Add(empID);
                passedVars.Add(sender.Text);
                //passedVars.Add("false");
                Debug.WriteLine(sender.Text);
                ContentFrame.Navigate(typeof(Samples.SamplesView), passedVars);
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
            if (result == ContentDialogResult.Primary) return;
            NavView.SelectedItem = null;
            Frame.Navigate(typeof(LoginWindow));
        }

        private async void Automatic_Signout()
        {
            ContentDialog autoSignOut = new ContentDialog
            {
                Title = "Idle",
                Content = "You have been idle for too long and have been signed out!",
                CloseButtonText = "Okay"
            };

            ContentDialogResult result = await autoSignOut.ShowAsync();
            Frame.Navigate(typeof(LoginWindow));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.Current.IsIdleChanged -= onIsIdleChanged;
        }

        private void onIsIdleChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"IsIdle: {App.Current.IsIdle}");
            if (App.Current.IsIdle) Automatic_Signout();
        }
    }
}