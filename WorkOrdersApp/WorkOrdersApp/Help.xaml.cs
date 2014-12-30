using System;
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
using WorkOrdersApp.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrdersApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Help : Page
    {
        public Help()
        {
            this.InitializeComponent();
            Webbrowser.NavigationStarting+=Webbrowser_NavigationStarting;
        }

        async void webView1_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
          
            // We turn around and hand the Uri to the system launcher to launch the default handler for it
            await Windows.System.Launcher.LaunchUriAsync(args.Uri);
            pageIsLoading = false;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            webaddress.Text = "http://google.com";
           
        }
        void address_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                NavigateToAddress(webaddress.Text);
            }
        }

        // used to navigate to the address given by forming the URL using the string URL given as a argument
        private void NavigateToAddress(string url)
        {
            try
            {

                Webbrowser.Navigate(new Uri(url));
            }
            catch (Exception ex)
            {
                Webbrowser.NavigateToString(String.Format("Address is invalid, try again.  Exception Message --> {0}", ex.Message));
            }
        }
        // navigates to the address present in the address box
        private void Go_Click(object sender, RoutedEventArgs e)
        {

            if (!pageIsLoading)
            {
                NavigateToAddress(webaddress.Text);
            }
            else
            {
                Webbrowser.Stop();
                pageIsLoading = false;
            }
        }
        private bool _pageIsLoading;
        // checks to see whether page is loading or not
        bool pageIsLoading
        {
            get { return _pageIsLoading; }
            set
            {
                _pageIsLoading = value;
                Go.Content = (value ? "Stop" : "Go");
                progressRing1.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);

                if (!value)
                {
                    Backward.IsEnabled = Webbrowser.CanGoBack;
                    Forward.IsEnabled = Webbrowser.CanGoForward;
                }
            }
        }


        private void Webbrowser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string url = "";
            try 
            {
                url = args.Uri.ToString();
            }
            finally
            {
                webaddress.Text = url;
               
                pageIsLoading = true;
            }
        }
        // goes to the previously browsed page on clicking the forward button
        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (Webbrowser.CanGoForward)
                Webbrowser.GoForward();
        }
        // goes to the previously browsed page on clicking the backward butto, we can navigate until home page, that is google page
        private void Backward_Click(object sender, RoutedEventArgs e)
        {
            if (Webbrowser.CanGoBack)
                Webbrowser.GoBack();
        }
        // returns to the split page
        private void Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Token.isFirstTime = "Map";
            this.Frame.Navigate(typeof(SplitPage1));
        }

    }
    
}
