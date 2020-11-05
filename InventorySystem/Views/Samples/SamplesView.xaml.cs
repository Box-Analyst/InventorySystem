using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace InventorySystem.Views.Samples
{
    public sealed partial class SamplesView : Page
    {
        private string empID;
        public SamplesView()
        {
            this.InitializeComponent();
            SizeChanged += new SizeChangedEventHandler(Page_SizeChanged);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            empID = e.Parameter?.ToString();
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
                    RowDefinition sampleRow = new RowDefinition();
                    sampleListGrid.RowDefinitions.Add(sampleRow);
                    sampleRow.Height = GridLength.Auto;

                    Button sampleButton = new Button
                    {
                        Content = sample,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 0, 2, 2),
                        Width = .7 * GetWidth()
                    };
                    Grid.SetRow(sampleButton, count);
                    Grid.SetColumn(sampleButton, 0);

                    Button recButton = new Button
                    {
                        Content = "Receive",
                        Width = .15 * GetWidth(),
                        Margin = new Thickness(0, 0, 2, 2)
                    };
                    Grid.SetRow(recButton, count);
                    Grid.SetColumn(recButton, 1);

                    Button distButton = new Button
                    {
                        Content = "Distribute",
                        Width = .15 * GetWidth(),
                        Margin = new Thickness(0, 0, 0, 2)
                    };
                    Grid.SetRow(distButton, count);
                    Grid.SetColumn(distButton, 2);

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
                    Content = sample,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 2, 2),
                    Width = .7 * GetWidth()
                };
                Grid.SetRow(sampleButton, count);
                Grid.SetColumn(sampleButton, 0);

                Button recButton = new Button
                {
                    Content = "Receive",
                    Width = .15 * GetWidth(),
                    Margin = new Thickness(0, 0, 2, 2)
                };
                Grid.SetRow(recButton, count);
                Grid.SetColumn(recButton, 1);

                Button distButton = new Button
                {
                    Content = "Distribute",
                    Width = .15 * GetWidth(),
                    Margin = new Thickness(0, 0, 0, 2)
                };
                Grid.SetRow(distButton, count);
                Grid.SetColumn(distButton, 2);

                sampleListGrid.Children.Add(sampleButton);
                sampleListGrid.Children.Add(recButton);
                sampleListGrid.Children.Add(distButton);
                count++;

            }

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