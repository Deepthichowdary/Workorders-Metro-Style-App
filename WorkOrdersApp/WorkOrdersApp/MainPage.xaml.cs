using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
using Salesforce.Force;
using Salesforce.Common;
using Salesforce.Common.Models;
using System.Threading.Tasks;
using WorkOrdersApp.Models;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrdersApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private  string SFAuthorizationEndpointUrl = "https://login.salesforce.com/services/oauth2/authorize";
        private string myConsumerKey = "3MVG9xOCXq4ID1uHC1doqqoPd_WllFJrDKwVBXdpngM9pbOa5dfsBom67oeoBteBStPDehGWmo2dEphXviF16";
        private  string myCallbackUrl = "sfdc://success";
        private String environment = "https://na17.salesforce.com";
        List<WorkOrderModel> WorkOrdrList = new List<WorkOrderModel>();
        List<dynamic> QueryResultList = new List<dynamic>();
       
        public MainPage()
        {
            this.InitializeComponent();
            Token.checkIn = false;
            
        }


        private async void login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get Token by connecting to Salesforce asynchronously 
                await GetToken();
                Token.firstTimeLogin = true;
               this.Frame.Navigate(typeof(SplitPage1));
            }
            catch (Exception ex)
            {
               String exception= ex.Message;
               
                
            }
           
        }
       
        // Get Access token by authenicating the user
        private async Task GetToken()
        {
            
            var token1 = new AuthToken();
            var app = (Application.Current as App);
            var startUrl = Common.FormatAuthUrl(SFAuthorizationEndpointUrl, ResponseTypes.Token, myConsumerKey,
                WebUtility.UrlEncode(myCallbackUrl), DisplayTypes.Popup);
            var startUri = new Uri(startUrl);
            var endUri = new Uri(CallbackUrl);
            var webAuthenticationResult =
                await Windows.Security.Authentication.Web.WebAuthenticationBroker.AuthenticateAsync(
                    Windows.Security.Authentication.Web.WebAuthenticationOptions.None,
                    startUri,
                    endUri);

            switch (webAuthenticationResult.ResponseStatus)
            {
                  // On success 
                case Windows.Security.Authentication.Web.WebAuthenticationStatus.Success:
                    var responseData = webAuthenticationResult.ResponseData;
                    var responseUri = new Uri(responseData);
                    var decoder = new WwwFormUrlDecoder(responseUri.Fragment.Replace("#", "?"));

                    // Save Access tokens for furthur use 
                    app.AccessToken = decoder.GetFirstValueByName("access_token");
                    Token.AccessToken = app.AccessToken;
                    app.RefreshToken = decoder.GetFirstValueByName("refresh_token");
                    Token.RefreshToken = app.RefreshToken;
                    app.InstanceUrl = WebUtility.UrlDecode(decoder.GetFirstValueByName("instance_url"));
                    Token.InstanceUrl = app.InstanceUrl;
                    Token.Id = decoder.GetFirstValueByName("id");

                    return;

                    // Throw the error 
                case Windows.Security.Authentication.Web.WebAuthenticationStatus.ErrorHttp:
                    throw new Exception(webAuthenticationResult.ResponseErrorDetail.ToString());

                default:
                    throw new Exception(webAuthenticationResult.ResponseData);
            }
        }

        // Refresh token to avoid token expiration
        private async Task RefreshToken()
        {
            var auth = new AuthenticationClient();
            await auth.TokenRefreshAsync(ConsumerKey, Token.RefreshToken);

            Token.AccessToken = auth.AccessToken;
        }

    }
}
