using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace InventorySystem.Views.Notifications
{
    internal static class Alerts
    {
        // Method to initialize alerts on startup
        public static void InitializeAlerts()
        {
            //
        }

        // Main alert function, pass what type(s) of alerts/info is needed to here.
        public static void CreateNewAlert(object sender, RoutedEventArgs e) { }

        // Spawns new notification with given title and body.
        public static void CreateNotification(string title, string body)
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
                    Text = title
                },
                new AdaptiveText()
                {
                    Text = body
                }
            }
                    }
                }
            };

            // Create the toast notification
            var toastNotify = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotify);
        }

        // Create a new entry in the alert pane.
        public static void CreateAlertEntry(object sender, RoutedEventArgs e) { }

        // Delete an entry from the alert pane.
        public static void DeleteAlertEntry(object sender, RoutedEventArgs e) { }
    }
}