using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InventorySystem.Views.Admin
{
    public sealed partial class AdminView : Page
    {
        public AdminView()
        {
            this.InitializeComponent();
            Output.ItemsSource = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", null);
        }

        private void Add_Text(object sender, RoutedEventArgs e)
        {
            SQL.ManageDB.Add_Text(sender, e, InputBox.Text);
            Output.ItemsSource = SQL.ManageDB.Grab_Entries("Sample", "NameandDosage", null);
        }
    }
}
