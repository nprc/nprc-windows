using System;
using AppStudio.ViewModels;
using AppStudio.Wat;
using Windows.ApplicationModel.DataTransfer;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
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

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            _mainViewModel = _mainViewModel ?? new MainViewModel();
            DataContext = this;
            ApplicationView.GetForCurrentView().
            SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);

            var splash = App.CurrentSplashScreen;
            if (splash != null)
            {
                // Optional: Add a progress ring to your splash screen to show users that content is loading
                PositionRing();
            }

            WatConfig.LoadConfigAsync(this.MainWebView, this.BottomCommandBar);
            this.MainWebView.NavigationCompleted += OnFirstLoadNavigationCompleted;
            HardwareButtons.BackPressed += OnBackPressed;
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

        /// <summary>
        /// Handles back button press.  If app is at the root page of app, don't go back and the
        /// system will suspend the app.
        /// </summary>
        /// <param name="sender">The source of the BackPressed event.</param>
        /// <param name="e">Details for the BackPressed event.</param>
        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            var RootFrame = Window.Current.Content as Frame;
            if (RootFrame == null)
            {
                return;
            }
            if (RootFrame.CanGoBack)
            {
                RootFrame.GoBack();
                e.Handled = true;
            }

            if (this.MainWebView.CanGoBack)
            {
                MainWebView.GoBack();
                e.Handled = true;
            }
        }

        #region "ExtendSplashScreen"
        void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width / 2.2);
            splashProgressRing.SetValue(Canvas.TopProperty, Window.Current.Bounds.Height / 5);
        }
        #endregion

        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
        }

        private void OnFirstLoadNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            this.MainWebView.Visibility = Visibility.Visible;
            this.loadingCanvas.Visibility = Visibility.Collapsed;
            this.BottomAppBar.Visibility = Visibility.Visible;

            this.MainWebView.NavigationCompleted -= OnFirstLoadNavigationCompleted;
            this._currentUri = args.Uri;

            this.MainWebView.NavigationStarting += OnNavigationStarting;
            this.MainWebView.NavigationCompleted += OnNavigationCompleted;

            _transferManager.DataRequested += OnDataRequested;
        }

        private async void OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            await statusBar.ProgressIndicator.ShowAsync();
        }

        private async void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            this._currentUri = args.Uri;
            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            await statusBar.ProgressIndicator.HideAsync();
        }
    }
}
