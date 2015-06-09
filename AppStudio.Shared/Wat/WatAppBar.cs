using AppStudio.Wat.Schema;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppStudio.Wat
{
    public class WatAppBar
    {
        private WatSchema _watConfig;
        private WebView _mainWebView;

        public WatAppBar(WatSchema watConfig, CommandBar bottomAppBar, WebView mainWebView)
        {
            _watConfig = watConfig;
            _mainWebView = mainWebView;

            // Add to current CommandBar in BottomAppBar Wat AppBarButtons if NavBar or AppBar is enabled           
            if ((_watConfig.NavBar == null) &&
                (_watConfig.AppBar == null))
            {
                return;
            }

            // Set the page's ApplicationBar
            var ApplicationBar = bottomAppBar;

            if (_watConfig.AppBar != null && _watConfig.AppBar.Buttons != null
                && _watConfig.AppBar.Buttons.Count <= 4)
            {
                // IconEnumHelper points to the appropriate files for enum-based     
                IconEnumHelper iconEnums = new IconEnumHelper();

                foreach (BarButton bb in _watConfig.AppBar.Buttons)
                {
                    AppBarButton appBarButton = new AppBarButton();

                    if (!String.IsNullOrEmpty(bb.Icon) && iconEnums.IsIconAvailable(bb.Icon))
                    {
                        appBarButton.Icon = new BitmapIcon()
                        {
                            UriSource = iconEnums.GetIconUri(bb.Icon)
                        };
                    }
                    else
                    {
                        appBarButton.Icon = new BitmapIcon()
                        {
                            UriSource = bb.IconUri
                        };
                    }
                    appBarButton.Label = bb.Label;
                    appBarButton.Click += appBarButton_Click;
                    ApplicationBar.PrimaryCommands.Add(appBarButton);
                }
            }

            if (_watConfig.NavBar != null && _watConfig.NavBar.Buttons != null)
            {
                foreach (BarButton bb in _watConfig.NavBar.Buttons)
                {
                    AppBarButton appBarMenuItem = new AppBarButton();
                    appBarMenuItem.Label = bb.Label;
                    appBarMenuItem.Click += appBarMenuItem_Click;
                    ApplicationBar.SecondaryCommands.Add(appBarMenuItem);
                }
            }
        }

        private void appBarMenuItem_Click(object sender, RoutedEventArgs e)
        {         
            if (_watConfig.NavBar != null && _watConfig.NavBar.Buttons != null)
            {
                AppBarButton item = (AppBarButton)sender;

                foreach (BarButton bb in _watConfig.NavBar.Buttons)
                {
                    if (bb.Label == item.Label)
                    {
                        if (Uri.IsWellFormedUriString(bb.ActionString, UriKind.Absolute))
                        {
                            _mainWebView.Navigate(bb.Action);
                            break;
                        }
                        else
                        {
                            if (bb.ActionString.Equals("home"))
                            {
                                if (_watConfig.MobileURLString != "" && Uri.IsWellFormedUriString(_watConfig.MobileURLString, UriKind.Absolute))
                                {
                                    _mainWebView.Navigate(_watConfig.MobileBaseURL);
                                }
                                else if (Uri.IsWellFormedUriString(_watConfig.BaseURLString, UriKind.Absolute))
                                {
                                    _mainWebView.Navigate(_watConfig.BaseURL);
                                }
                                break;
                            }
                            else if (bb.ActionString.Equals("refresh"))
                            {
                                _mainWebView.Refresh();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void appBarButton_Click(object sender, RoutedEventArgs e)
        {           
            if (_watConfig.AppBar != null && _watConfig.AppBar.Buttons != null)
            {
                AppBarButton button = (AppBarButton)sender;

                foreach (BarButton bb in _watConfig.AppBar.Buttons)
                {
                    if (bb.Label == button.Label)
                    {
                        if (Uri.IsWellFormedUriString(bb.ActionString, UriKind.Absolute))
                        {
                            _mainWebView.Navigate(bb.Action);
                            break;
                        }
                        else
                        {
                            if (bb.ActionString.Equals("home"))
                            {
                                if (_watConfig.MobileURLString != "" && Uri.IsWellFormedUriString(_watConfig.MobileURLString, UriKind.Absolute))
                                {
                                    _mainWebView.Navigate(_watConfig.MobileBaseURL);                                    
                                }
                                else if (Uri.IsWellFormedUriString(_watConfig.BaseURLString, UriKind.Absolute))
                                {
                                    _mainWebView.Navigate(_watConfig.BaseURL);
                                }
                                break;
                            }
                            else if (bb.ActionString.Equals("refresh"))
                            {
                                _mainWebView.Refresh();
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}
