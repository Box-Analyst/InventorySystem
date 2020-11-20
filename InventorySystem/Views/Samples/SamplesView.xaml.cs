#region copyright

// Copyright (c) Box Analyst. All rights reserved.
// This code is licensed under the GNU AGPLv3 License.

#endregion copyright

using InventorySystem.Views.Samples.Components;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace InventorySystem.Views.Samples
{
    public sealed partial class SamplesView : Page
    {
        private string empID, currentSampleNo, currentSampleName, currentSampleExpDate, NameandDosageText;
        private bool isSampleSearched = false;
        private int currentSampleCount;
        private List<string> passedVars = new List<string>();
        private static List<Sample> SampleList = new List<Sample>();
        private static List<Sample> SearchedSample = new List<Sample>();

        public SamplesView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passedVars.Clear();
            if (e.Parameter is string)
            {
                empID = e.Parameter.ToString();
            }
            else
            {
                passedVars = e.Parameter as List<string>;
                empID = passedVars?[0];
                NameandDosageText = passedVars?[1];
                isSampleSearched = true;
                passedVars?.Clear();
            }
            passedVars?.Add(empID);
            SampleList.Clear();
            ConstructSamplesList();
            if (isSampleSearched)
            {
                OnSearchedSamplesList();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void ConstructSamplesList()
        {
            List<string> SampleNameDose = new List<string>();
            if (NameandDosageText != null)
            {
                SampleList = SampleDataSource.GetSearchedSample(NameandDosageText);
                if (SampleList.Count != 0)
                {
                    foreach (Sample s in SampleList)
                    {
                        SampleNameDose.Add(s.NameandDosage);
                    }
                }
                else
                {
                    isSampleSearched = false;
                    SampleList = SampleDataSource.GetSamples();
                    foreach (Sample s in SampleList)
                    {
                        SampleNameDose.Add(s.NameandDosage);
                    }
                }
            }
            else
            {
                SampleList = SampleDataSource.GetSamples();
                foreach (Sample s in SampleList)
                {
                    SampleNameDose.Add(s.NameandDosage);
                }
            }
            if (SQL.ManageDB.IsEmpty()) return;
            sampleListView.ItemsSource = SampleNameDose;

            sampleListView.ItemClick += SampleClick;
        }

        private void OnSearchedSamplesList()
        {
            SearchedSample = SampleDataSource.GetSearchedSample(NameandDosageText);
            currentSampleNo = SearchedSample[0].LotNum;
            currentSampleName = SearchedSample[0].NameandDosage;
            currentSampleCount = SearchedSample[0].Count;
            currentSampleExpDate = SearchedSample[0].ExpirationDate;
            DisplaySampleInformation();
            isSampleSearched = false;
        }

        public void SampleClick(object sender, ItemClickEventArgs e)
        {
            ClearDetailsPanel();
            ListView samplesList = (ListView)sender;
            string clickedMenuItem = (string)e.ClickedItem;

            if (samplesList.Items != null)
            {
                int itemNum = samplesList.Items.IndexOf(clickedMenuItem);
                currentSampleNo = SampleList[itemNum].LotNum;
                currentSampleName = SampleList[itemNum].NameandDosage;
                currentSampleCount = SampleList[itemNum].Count;
                currentSampleExpDate = SampleList[itemNum].ExpirationDate;
            }

            DisplaySampleInformation();
        }

        private void DisplaySampleInformation()
        {
            TextBlock nameDoseText = new TextBlock
            {
                Name = "nameDose",
                Text = currentSampleName,
                FontSize = 24,
                Margin = new Thickness(0, 0, 0, 25)
            };
            TextBlock lotNumText = new TextBlock { Text = "Lot Number: " + currentSampleNo, FontSize = 16 };
            TextBlock countText = new TextBlock { Text = "Count: " + currentSampleCount, FontSize = 16 };
            TextBlock expDateText = new TextBlock { Text = "Expiration Date: " + currentSampleExpDate, FontSize = 16 };

            StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
            Button recButton = new Button
            {
                Name = "recButton",
                Content = "Receive",
                Margin = new Thickness(0, 20, 4, 0),
            };
            recButton.Click += RecButton_Click;

            Button distButton = new Button
            {
                Name = "distButton",
                Content = "Distribute",
                Margin = new Thickness(4, 20, 0, 0),
            };
            distButton.Click += DistButton_Click;

            buttonPanel.Children.Add(recButton);
            buttonPanel.Children.Add(distButton);

            detailsPanel.Children.Add(nameDoseText);
            detailsPanel.Children.Add(lotNumText);
            detailsPanel.Children.Add(countText);
            detailsPanel.Children.Add(expDateText);
            detailsPanel.Children.Add(buttonPanel);
        }

        private void RecButton_Click(object sender, RoutedEventArgs e)
        {
            passedVars.Add(currentSampleNo);
            passedVars.Add(currentSampleName);
            Frame.Navigate(typeof(AddSample), passedVars);
        }

        private void DistButton_Click(object sender, RoutedEventArgs e)
        {
            if (HasClearance())
            {
                passedVars.Add(currentSampleNo);
                passedVars.Add(currentSampleName);
                Frame.Navigate(typeof(DistributeSample), passedVars);
            }
            else
            {
                DisplayNoPrivilege();
            }
        }

        private async void DisplayNoPrivilege()
        {
            ContentDialog noPrivilege = new ContentDialog
            {
                Title = "Insufficient Privileges",
                Content = "This account does not have the privileges associated with distributing samples to patients.",
                CloseButtonText = "Ok"
            };
            await noPrivilege.ShowAsync();
        }

        private void ClearDetailsPanel()
        {
            detailsPanel.Children.Clear();
        }

        private bool HasClearance()
        {
            string privLevel = SQL.ManageDB.Grab_Entries("Login", "PrivLevel", "Emp_id", empID)[0];
            return (privLevel == "0" || privLevel == "1");
        }
    }
}