﻿using System;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InventorySystem.Views.Home.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotifyPane : Page
    {
        public NotifyPane()
        {
            this.InitializeComponent();
            notifyText();
        }

        private void expiryList()
        {
            //
        }

        private void expireSoonList()
        {
            //for (int i = 0; i < entries.Count; i++)
            //{
            //    if (SQL.ManageDB.Check_IsExpired(entries[i]))
            //    {
            //    }
            //}
        }

        private void notifyText()
        {
            expiryAlert.Text = "Placeholder.";
        }
    }
}
