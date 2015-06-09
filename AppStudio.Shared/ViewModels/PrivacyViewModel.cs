using System;
using AppStudio.Data;
using Windows.ApplicationModel;

namespace AppStudio.ViewModels
{
    public class PrivacyViewModel : BindableBase
    {
        public Uri Url
        {
            get
            {
                return new Uri(UrlText, UriKind.RelativeOrAbsolute);
            }
        }
        public string UrlText
        {
            get
            {
                return "http://nprc.nz/privacy.html";
            }
        }
    }
}

