using System;
using System.Windows.Input;
using AppStudio.Data;
using AppStudio.Services;
using AppStudio.Wat;
using Windows.ApplicationModel.DataTransfer;

namespace AppStudio.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private PrivacyViewModel _privacyModel;


        public MainViewModel()
        {
            _privacyModel = new PrivacyViewModel();
        }


        public ICommand AboutCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    NavigationServices.NavigateToPage("AboutThisAppPage");
                });
            }
        }
        public ICommand PrivacyCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    NavigationServices.NavigateTo(_privacyModel.Url);
                });
            }
        }

        public ICommand ShareCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        public void GetDataShare(DataRequest request, Uri currentUri)
        {
            if (WatConfig.watConfig.Share.IsEnabled && !string.IsNullOrEmpty(WatConfig.watConfig.Share.Message))
            {
                string message = WatConfig.watConfig.Share.Message;
                if (message.IndexOf("{url}") >= 0)
                {
                    string shareUrl = WatConfig.watConfig.Share.ShareUrl;
                    if (!string.IsNullOrEmpty(shareUrl))
                    {
                        string url = string.Empty;
                        if (shareUrl.ToLower() == "{currenturl}")
                        {
                            url = currentUri.OriginalString;
                        }
                        else if (Uri.IsWellFormedUriString(shareUrl, UriKind.Absolute))
                        {
                            url = shareUrl;
                        }

                        message = message.Replace("{url}", url);
                    }
                }
                request.Data.Properties.Title = WatConfig.watConfig.Share.Title;
                request.Data.SetText(message);
            }
        }
    }
}
