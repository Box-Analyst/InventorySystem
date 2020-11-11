using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.WindowManagement;
using System.Security.Cryptography;
using Windows.UI.Core;
using System.Windows;
using System.Text.RegularExpressions;
using InventorySystem.Views.Shell;
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;
using System.Collections.Generic;
using System.Diagnostics;
using InventorySystem.Views.Samples.Components;

namespace InventorySystem.Views.Samples
{
    public sealed partial class SamplesView : Page
    {
        private string empID, currentSampleNo, currentSampleName, currentSampleExpDate, NameandDosageText;
        private string isSampleSearched = "false";
        private int currentSampleCount;
        private List<string> passedVars = new List<string>();
        private static List<Sample> SampleList = new List<Sample>();
        private static List<Sample> SearchedSample = new List<Sample>();
        public SamplesView()
        {
            this.InitializeComponent();
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
                empID = passedVars[0];
                NameandDosageText = passedVars?[1];
                isSampleSearched = passedVars[2];
                passedVars.Clear();
            }
            passedVars.Add(empID);
            SampleList.Clear();
            ConstructSamplesList();
            if(isSampleSearched == "true")
            {
                OnSearchedSamplesList();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) { }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) { }

        private void ConstructSamplesList()
        {
            List<string> SampleNameDose = new List<string>();
            if (NameandDosageText != null)
            {
                SampleList = SampleDataSource.GetSearchedSample(NameandDosageText);
                foreach (Sample s in SampleList)
                {
                    SampleNameDose.Add(s.NameandDosage);
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

            sampleListView.ItemClick += new ItemClickEventHandler(SampleClick);
        }

        private void OnSearchedSamplesList()
        {
            SearchedSample = SampleDataSource.GetSearchedSample(NameandDosageText);
            currentSampleNo = SearchedSample[0].LotNum;
            currentSampleName = SearchedSample[0].NameandDosage;
            currentSampleCount = SearchedSample[0].Count;
            currentSampleExpDate = SearchedSample[0].ExpirationDate;
            DisplaySampleInformation();



        }
        public void SampleClick(object sender, ItemClickEventArgs e)
        {
            ClearDetailsPanel();
            ListView samplesList = (ListView)sender;
            string clickedMenuItem = (string)e.ClickedItem;

            int itemNum = samplesList.Items.IndexOf(clickedMenuItem);
            currentSampleNo = SampleList[itemNum].LotNum;
            currentSampleName = SampleList[itemNum].NameandDosage;
            currentSampleCount = SampleList[itemNum].Count;
            currentSampleExpDate = SampleList[itemNum].ExpirationDate;
            DisplaySampleInformation();
        }

        private void DisplaySampleInformation()
        {
            TextBlock nameDoseText = new TextBlock();
            nameDoseText.Name = "nameDose";
            nameDoseText.Text = currentSampleName;
            nameDoseText.FontSize = 24;
            nameDoseText.Margin = new Thickness(0, 0, 0, 25);
            TextBlock lotNumText = new TextBlock();
            lotNumText.Text = "Lot Number: " + currentSampleNo;
            lotNumText.FontSize = 16;
            TextBlock countText = new TextBlock();
            countText.Text = "Count: " + currentSampleCount;
            countText.FontSize = 16;
            TextBlock expDateText = new TextBlock();
            expDateText.Text = "Expiration Date: " + currentSampleExpDate;
            expDateText.FontSize = 16;

            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;
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
            passedVars.Add(currentSampleNo);
            passedVars.Add(currentSampleName);
            Frame.Navigate(typeof(DistributeSample), passedVars);
        }
        private void ClearDetailsPanel()
        {
                detailsPanel.Children.Clear();
        }

    }
}