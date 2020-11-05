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

namespace InventorySystem.Views.Samples
{
    public sealed partial class SamplesView : Page
    {
        private string empID, currentSample;
        private List<string> passedVars = new List<string>();
        public SamplesView()
        {
            this.InitializeComponent();
            SizeChanged += new SizeChangedEventHandler(Page_SizeChanged);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter?.ToString();
            passedVars.Clear();
            passedVars.Add(GetEmpID());
            ConstructSamplesList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((Math.Abs(e.NewSize.Width - e.PreviousSize.Width)) > 0)
            {
                UpdateSamplesList();
            }
        }

        private void ConstructSamplesList()
        {
            var samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", null);
            int numRows = SQL.ManageDB.NumberOfRows();
            int count = 0;
            if (numRows != 0)
            {
                ColumnDefinition sampleCol = new ColumnDefinition();
                ColumnDefinition recCol = new ColumnDefinition();
                ColumnDefinition distCol = new ColumnDefinition();
                sampleCol.Width = new GridLength(7, GridUnitType.Star);
                recCol.Width = new GridLength(1.5, GridUnitType.Star);
                distCol.Width = new GridLength(1.5, GridUnitType.Star);
                sampleListGrid.ColumnDefinitions.Add(sampleCol);
                sampleListGrid.ColumnDefinitions.Add(recCol);
                sampleListGrid.ColumnDefinitions.Add(distCol);

                foreach (string sample in samples)
                {
                    currentSample = sample;
                    RowDefinition sampleRow = new RowDefinition();
                    sampleListGrid.RowDefinitions.Add(sampleRow);
                    sampleRow.Height = GridLength.Auto;

                    Button sampleButton = new Button
                    {
                        Name = "sampleButton" + count,
                        Content = sample,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 0, 2, 2),
                        Width = .7 * GetWidth()
                    };
                    Grid.SetRow(sampleButton, count);
                    Grid.SetColumn(sampleButton, 0);
                    sampleButton.Click += new RoutedEventHandler(SampleButton_Click);

                    Button recButton = new Button
                    {
                        Name = "recButton" + count,
                        Content = "Receive",
                        Width = .15 * GetWidth(),
                        Margin = new Thickness(0, 0, 2, 2)
                    };
                    Grid.SetRow(recButton, count);
                    Grid.SetColumn(recButton, 1);
                    recButton.Click += RecButton_Click;

                    Button distButton = new Button
                    {
                        Name = "distButton" + count,
                        Content = "Distribute",
                        Width = .15 * GetWidth(),
                        Margin = new Thickness(0, 0, 0, 2)
                    };
                    Grid.SetRow(distButton, count);
                    Grid.SetColumn(distButton, 2);
                    distButton.Click += new RoutedEventHandler(DistButton_Click);

                    sampleListGrid.Children.Add(sampleButton);
                    sampleListGrid.Children.Add(recButton);
                    sampleListGrid.Children.Add(distButton);
                    count++;
                }
            }
        }

        //Not working/saving for later ---DONT MERGE---
        /*private void UpdateSamplesList()
        {
            foreach (var button in from RowDefinition row in sampleListGrid.RowDefinitions
                                   from UIElement button in sampleListGrid.Children
                                   select button)
            {
                Debug.WriteLine(button.ToString());
                switch (Content)
                {
                    case "Receive":
                        Button rbtn = (Button)button;
                        rbtn.Width = .15 * ((Frame)Window.Current.Content).ActualWidth;
                        rbtn.Content = "Receive";
                        break;
                    case "Distribute":
                        Button dbtn = (Button)button;
                        dbtn.Width = .15 * ((Frame)Window.Current.Content).ActualWidth;
                        dbtn.Content = "Distribute";
                        break;
                    default:
                        Button btn = (Button)button;
                        btn.Width = .7 * ((Frame)Window.Current.Content).ActualWidth;
                        break;
                }
            }
        }*/

        //Working, but slow/memory heavy
        //Updates the Sample list by calling ClearContent, and rebuilding the grid's rows using preexisting columns
        private void UpdateSamplesList()
        {
            ClearContent();
            var samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", null);
            int count = 0;
            foreach (string sample in samples)
            {
                RowDefinition sampleRow = new RowDefinition();
                sampleListGrid.RowDefinitions.Add(sampleRow);
                sampleRow.Height = GridLength.Auto;

                Button sampleButton = new Button
                {
                    Name = "sampleButton" + count,
                    Content = sample,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 2, 2),
                    Width = .7 * GetWidth()
                };
                Grid.SetRow(sampleButton, count);
                Grid.SetColumn(sampleButton, 0);
                sampleButton.Click += new RoutedEventHandler(SampleButton_Click);

                Button recButton = new Button
                {
                    Name = "recButton" + count,
                    Content = "Receive",
                    Width = .15 * GetWidth(),
                    Margin = new Thickness(0, 0, 2, 2)
                };
                Grid.SetRow(recButton, count);
                Grid.SetColumn(recButton, 1);
                recButton.Click += new RoutedEventHandler(RecButton_Click);

                Button distButton = new Button
                {
                    Name = "distButton" + count,
                    Content = "Distribute",
                    Width = .15 * GetWidth(),
                    Margin = new Thickness(0, 0, 0, 2)
                };
                Grid.SetRow(distButton, count);
                Grid.SetColumn(distButton, 2);
                distButton.Click += new RoutedEventHandler(DistButton_Click);

                sampleListGrid.Children.Add(sampleButton);
                sampleListGrid.Children.Add(recButton);
                sampleListGrid.Children.Add(distButton);
                count++;

            }

        }


        //Onclick event that occurs if the Sample number is clicked
        //Will navigate to a sample information page and pass all relevant data via passedVars
        private void SampleButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SamplesView), GetEmpID());
        }

        //Onclick event that occurs if the Receive button is clicked
        //Will navigate to the AddSample page with data based on
        //which sample's button was pressed.
        private void RecButton_Click(object sender, RoutedEventArgs e)
        {
            var samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", null);
            var names_dose = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", null);
            //Grabs the button's name property and extracts the integer (based on count)
            string resultString = Regex.Match(((Button)sender).Name, @"\d+").Value;
            int count = int.Parse(resultString);
            currentSample = samples[count];
            passedVars.Add(samples[count]);
            passedVars.Add(names_dose[count]);
            //Debug.WriteLine(currentSample);
            Frame.Navigate(typeof(Components.AddSample), passedVars);
        }

        //Onclick event that occurs if the Distribute button is clicked
        //Will navigate to the DistributeSample page with data based on
        //which sample's button was pressed.
        private void DistButton_Click(object sender, RoutedEventArgs e)
        {
            var samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", null);
            var names_dose = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", null);
            string resultString = Regex.Match(((Button)sender).Name, @"\d+").Value;
            int count = int.Parse(resultString);
            currentSample = samples[count];
            passedVars.Add(currentSample);
            passedVars.Add(names_dose[count]);
            //Debug.WriteLine(currentSample);
            Frame.Navigate(typeof(Components.DistributeSample), passedVars);
        }
        //Deletes the Grid's rows and content within, leaving the columns for use in rebuilding the list
        private void ClearContent()
        {
            foreach (RowDefinition row in sampleListGrid.RowDefinitions)
            {
                foreach (UIElement control in sampleListGrid.Children)
                {
                    sampleListGrid.Children.Remove(control);
                }
                sampleListGrid.RowDefinitions.Remove(row);
            }

        }

        //Returns the current window's actualwidth - used for button width calculations
        public double GetWidth()
        {
            return ((Frame)Window.Current.Content).ActualWidth;
        }
        public string GetEmpID()
        {
            return empID;
        }
    }
}