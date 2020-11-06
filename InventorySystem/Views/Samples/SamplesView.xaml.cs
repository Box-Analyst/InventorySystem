﻿using System;
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

namespace InventorySystem.Views.Samples
{
    public sealed partial class SamplesView : Page
    {
        private string empID, currentSample, NameandDosageText;
        private List<string> passedVars = new List<string>();
        public SamplesView()
        {
            this.InitializeComponent();
            SizeChanged += new SizeChangedEventHandler(Page_SizeChanged);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passedVars.Clear();
            passedVars.Add(empID);
            if (e.Parameter is string)
            {
                empID = e.Parameter.ToString();
            }
            else
            {
                passedVars = e.Parameter as List<string>;
                empID = passedVars[0];
                NameandDosageText = passedVars?[1];
                passedVars.Clear();
            }
            passedVars.Add(empID);
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
            ClearContent();
            List<string> samples;
            if(NameandDosageText != null)
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "NameandDosage", NameandDosageText);
            } else
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "isExpired", 0);
            }

            int numRows = SQL.ManageDB.NumberOfRows();
            int count = 1;
            if (numRows == 0) return;
            ColumnDefinition sampleCol = new ColumnDefinition();
            ColumnDefinition recCol = new ColumnDefinition();
            ColumnDefinition distCol = new ColumnDefinition();
            sampleCol.Width = new GridLength(8.8, GridUnitType.Star);
            recCol.Width = new GridLength(0.6, GridUnitType.Star);
            distCol.Width = new GridLength(0.6, GridUnitType.Star);
            sampleListGrid.ColumnDefinitions.Add(sampleCol);
            sampleListGrid.ColumnDefinitions.Add(recCol);
            sampleListGrid.ColumnDefinitions.Add(distCol);

            RowDefinition headerRow = new RowDefinition();
            sampleListGrid.RowDefinitions.Add(headerRow);

            TextBlock header = new TextBlock();
            header.Text = "Sample Lot Numbers";
            Grid.SetRow(header, 0);
            Grid.SetColumn(header, 0);

            TextBlock recHeader = new TextBlock();
            recHeader.Text = "Receive";
            recHeader.TextAlignment = TextAlignment.Center;
            header.Margin = new Thickness(0, 0, 0, 4);
            Grid.SetRow(recHeader, 0);
            Grid.SetColumn(recHeader, 1);

            TextBlock distHeader = new TextBlock();
            distHeader.Text = "Distribute";
            distHeader.TextAlignment = TextAlignment.Center;
            distHeader.Margin = new Thickness(0, 0, 0, 4);
            Grid.SetRow(distHeader, 0);
            Grid.SetColumn(distHeader, 2);

            sampleListGrid.Children.Add(header);
            sampleListGrid.Children.Add(recHeader);
            sampleListGrid.Children.Add(distHeader);

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
                    Width = .88 * GetWidth()
                };
                Grid.SetRow(sampleButton, count);
                Grid.SetColumn(sampleButton, 0);
                sampleButton.Click += new RoutedEventHandler(SampleButton_Click);

                Button recButton = new Button
                {
                    Name = "recButton" + count,
                    Content = "+",
                    Width = .06 * GetWidth(),
                    Margin = new Thickness(0, 0, 2, 2)
                };
                Grid.SetRow(recButton, count);
                Grid.SetColumn(recButton, 1);
                recButton.Click += RecButton_Click;

                Button distButton = new Button
                {
                    Name = "distButton" + count,
                    Content = "–",
                    Width = .06 * GetWidth(),
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

        //Updates the Sample list by calling ClearContent, and rebuilding the grid's rows using preexisting columns
        private void UpdateSamplesList()
        {
            ClearContent();
            List<string> samples;
            if (NameandDosageText != null)
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "NameandDosage", NameandDosageText);
            }
            else
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "isExpired", 0);
            }
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
                    Width = .88 * GetWidth()
                };
                Grid.SetRow(sampleButton, count);
                Grid.SetColumn(sampleButton, 0);
                sampleButton.Click += new RoutedEventHandler(SampleButton_Click);

                Button recButton = new Button
                {
                    Name = "recButton" + count,
                    Content = "+",
                    Width = .06 * GetWidth(),
                    Margin = new Thickness(0, 0, 2, 2)
                };
                Grid.SetRow(recButton, count);
                Grid.SetColumn(recButton, 1);
                recButton.Click += new RoutedEventHandler(RecButton_Click);

                Button distButton = new Button
                {
                    Name = "distButton" + count,
                    Content = "–",
                    Width = .06 * GetWidth(),
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
            Frame.Navigate(typeof(SamplesView), empID);
        }

        //Onclick event that occurs if the Receive button is clicked
        //Will navigate to the AddSample page with data based on
        //which sample's button was pressed.
        private void RecButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> samples;
            List<string> names_dose;
            if (NameandDosageText != null)
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "NameandDosage", NameandDosageText);
                names_dose = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", "NameandDosage", NameandDosageText);
            }
            else
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "isExpired", 0);
                names_dose = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", "isExpired", 0);
            }
            //Grabs the button's name property and extracts the integer (based on count)
            string resultString = Regex.Match(((Button)sender).Name, @"\d+").Value;
            int count = int.Parse(resultString) - 1;
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
            List<string> samples;
            List<string> names_dose;
            if (NameandDosageText != null)
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "NameandDosage", NameandDosageText);
                names_dose = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", "NameandDosage", NameandDosageText);
            }
            else
            {
                samples = SQL.ManageDB.Grab_Entries("Sample", "LotNum", "isExpired", 0);
                names_dose = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", "isExpired", 0);
            }
            string resultString = Regex.Match(((Button)sender).Name, @"\d+").Value;
            int count = int.Parse(resultString) - 1;
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
    }
}