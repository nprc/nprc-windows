using AppStudio.ToastNotifications.ToastContent;
using Windows.UI.Notifications;

namespace AppStudio.ToastNotifications
{
    public static class DisplayPackageImageToast
    {        
        public static void Display(string notificationText)
        {
            IToastNotificationContent toastContent = null;

            //Change ToastContentFactory.Template to try other toast types
            IToastText01 templateContent = ToastContentFactory.CreateToastText01();
            templateContent.TextBodyWrap.Text = notificationText;           
            toastContent = templateContent;

            // Create a toast from the Xml, then create a ToastNotifier object to show
            // the toast
            ToastNotification toast = toastContent.CreateNotification();

            // If you have other applications in your package, you can specify the AppId of
            // the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
   
}
