using System;
using AppStudio.ViewModels;
using AppStudio.Wat;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppStudio.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel _mainViewModel = null;
        private DataTransferManager _transferManager = null;
        private Uri _currentUri = null;

        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        Boolean navigationByRefresh = false;
        Boolean isHomePage = true;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            _mainViewModel = _mainViewModel ?? new MainViewModel();
            DataContext = this;

            WatConfig.LoadConfigAsync(this.MainWebView, this.BottomCommandBar);

            this.MainWebView.NavigationCompleted += OnFirstLoadNavigationCompleted;

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);

            splash = App.CurrentSplashScreen;
            if (splash != null)
            {
                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();

                // Optional: Add a progress ring to your splash screen to show users that content is loading
                PositionRing();


            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _transferManager = _transferManager ?? DataTransferManager.GetForCurrentView();
            _transferManager.DataRequested += OnDataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _transferManager.DataRequested -= OnDataRequested;
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _mainViewModel.GetDataShare(args.Request, _currentUri);
        }

        #region "ExtendSplashScreen"
        // Position the extended splash screen image in the same location as the system splash screen image.
        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;

        }

        void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.01));
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }
        #endregion

        private void OnFirstLoadNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            this.MainWebView.Visibility = Visibility.Visible;
            this.loadingCanvas.Visibility = Visibility.Collapsed;
            this.MainWebView.NavigationCompleted -= OnFirstLoadNavigationCompleted;
            this._currentUri = args.Uri;
            this.MainWebView.NavigationStarting += OnNavigationStarting;
            this.MainWebView.NavigationCompleted += OnNavigationCompleted;
            _transferManager.DataRequested += OnDataRequested;
        }

        private void OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            navigationByRefresh = IsFromRefreshAction(sender.Source, args.Uri);
            isHomePage = IsHomePage(args.Uri);

            if (!navigationByRefresh && !isHomePage)
            {
                this.MainWebViewTitle.Visibility = Visibility.Collapsed;
                this.TopNavigationAppBar.Visibility = Visibility.Visible;
                this.navigationProgressRing.Visibility = Visibility.Visible;
                this.GoBackButton.Visibility = Visibility.Visible;
                this.TopNavigationAppBar.IsOpen = true;
            }
            else
            {
                this.MainWebViewTitle.Visibility = Visibility.Collapsed;
                this.TopNavigationAppBar.Visibility = Visibility.Visible;
                this.navigationProgressRing.Visibility = Visibility.Visible;
                this.GoBackButton.Visibility = Visibility.Collapsed;
                this.TopNavigationAppBar.IsOpen = true;
            }
        }

        private void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            this._currentUri = args.Uri;
            if (!navigationByRefresh && !isHomePage)
            {
                this.navigationProgressRing.Visibility = Visibility.Collapsed;
                this.TopNavigationAppBar.IsOpen = false;
                this.MainWebViewTitle.Visibility = Visibility.Visible;
                this.MainWebViewTitle.Text = this.MainWebView.DocumentTitle;
            }
            else
            {
                this.TopNavigationAppBar.IsOpen = false;
                this.TopNavigationAppBar.Visibility = Visibility.Collapsed;
            }

            if (this.BottomAppBar.IsOpen)
            {
                this.BottomAppBar.IsOpen = false;
            }
        }

        private Boolean IsFromRefreshAction(Uri navigationFromUri, Uri navigationToUri)
        {
            return navigationFromUri == navigationToUri;
        }

        private Boolean IsHomePage(Uri navigationToUri)
        {
            return WatConfig.watConfig.BaseURL.ToString() == navigationToUri.ToString();
        }

        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.MainWebView.CanGoBack)
            {
                this.MainWebView.GoBack();
            }

            if (!this.MainWebView.CanGoBack)
            {
                this.TopNavigationAppBar.Visibility = Visibility.Collapsed;
                this.TopNavigationAppBar.IsOpen = false;
            }

            if (this.BottomAppBar.IsOpen)
            {
                this.BottomAppBar.IsOpen = false;
            }
        }
    }
}
