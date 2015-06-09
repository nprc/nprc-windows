using AppStudio.ToastNotifications;
using AppStudio.Wat.Schema;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace AppStudio.Wat
{
    [DataContract]
    public static class WatConfig
    {
        private const string WAT_CONFIG_PATH = "ms-appx:///Wat/config.json";
        public static WatSchema watConfig;
        public static WebView mainWebView;

        public static async void LoadConfigAsync(WebView currentWebView, CommandBar bottomAppBar)
        {
            ResourceLoader resourceLoader = new ResourceLoader();
            watConfig = new WatSchema();
            mainWebView = currentWebView;
          
            //Load WatConfig
            try
            {
                watConfig = await WatConfig.GetWatConfigAsync();
                mainWebView.Navigate(new Uri(watConfig.BaseURLString));
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("WatConfigLoadWatConfig", ex);
                Debug.WriteLine(ex);
                DisplayPackageImageToast.Display(resourceLoader.GetString("LoadError"));
              
#if WINDOWS_PHONE_APP
                App.Current.Exit();
#endif
            }

            //Load AppBar
            try
            {
                if (watConfig != null)
                {
                    var watAppBar = new WatAppBar(watConfig, bottomAppBar, mainWebView);
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("WatConfigLoadAppBar", ex);
                Debug.WriteLine(ex);
                DisplayPackageImageToast.Display(resourceLoader.GetString("LoadAppBarError"));
            }
                   
            RegisterForShare();
        }

        private static async Task<WatSchema> GetWatConfigAsync()
        {
            // Get a file from the installation folder with the ms-appx URI scheme.
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(WAT_CONFIG_PATH));
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string configJSON = sr.ReadToEnd();
                    return WatSchema.CreateConfig(configJSON);
                }
            }
        }
        
        private static void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareLinkHandler);
        }

        private static void ShareLinkHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            // The Title is mandatory
            request.Data.Properties.Title = watConfig.Share.Title;

            string shareUrl = string.Empty;
            if (watConfig.Share.ShareUrl == "{currentURL}")
            {
                shareUrl = mainWebView.Source.ToString();
                request.Data.SetWebLink(mainWebView.Source);
            }
            else
            {
                shareUrl = watConfig.Share.ShareUrl;
                request.Data.SetWebLink(new Uri(shareUrl));
            }
            request.Data.Properties.Description = watConfig.Share.Message.Replace("{url}", shareUrl);
        }

    }
}
