using WorkOrdersApp.AppCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkOrdersApp.Models;
using Salesforce.Force;
using Salesforce.Common;
using Salesforce.Common.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Storage;
using System.Text;
using WorkOrdersApp.ViewModels;
using Windows.ApplicationModel.Activation;
using SQLite;
using Windows.Networking.Connectivity;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;







// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace WorkOrdersApp
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class SplitPage1 : Page
    {
              
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        List<WorkOrderModel> WOListObj = new List<WorkOrderModel>();
        List<dynamic> QueryResultList = new List<dynamic>();
        int Selected_ListIndex;
        private string htmlString;
        NetworkStatusChangedEventHandler networkStatusCallback;
        Boolean registeredNetworkStatusNotification = false;
        
        
        public async void updateView()
        {
            WorkOrderModel mWorkOrderModel = new WorkOrderModel();
            string json = await GetJSONString();
            WorkOrderList.WorkOrdersList = JsonConvert.DeserializeObject<List<WorkOrderModel>>(json);
            itemListView.ItemsSource = null;
            itemListView.ItemsSource = WorkOrderList.WorkOrdersList;
            

            
        }
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

      // Fetch details from database
        public async Task<String> GetJSONString()
        {
            string clientId = Token.Id;
            string mclientID = clientId.Split(new char[] { @"\"[0], "/"[0] }).Last();
            string wListURL = Token.InstanceUrl + "/services/apexrest/MyRestPullWorkOrderList/" + mclientID;
            HttpClient client = new HttpClient();
            HttpWebRequest request = HttpWebRequest.Create(wListURL) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";
            request.Headers["Authorization"] = "Bearer " + Token.AccessToken;
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            //var responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());

            String st1 = streamReader.ReadToEnd();
            return st1;
      
        }

       
        
        //On click Logout
        private async void Click_Logout(object sender, RoutedEventArgs e)
        {
            try
            {
                Token.AccessToken = null;
                Token.RefreshToken = null;
                Token.InstanceUrl = null;
                this.Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
            }


        }

        


        public SplitPage1()
        {
            this.InitializeComponent();
            GPS.IsTapEnabled = false;
            buttonWithFlyout1.IsEnabled = false;
            Camera.IsTapEnabled = false;
            done.IsTapEnabled = false;
            esig.IsTapEnabled = false;
            if(Token.checkIn == true)
            {
                buttonWithFlyout1.IsEnabled = true;
                Camera.IsTapEnabled = true;
                GPS.IsTapEnabled = false;
                Token.checkIn = false;
                

            }
            if(Token.firstTimeLogin == true)
            {
                Token.IdOld = null;
                Token.firstTimeLogin = false;
            }
           
            this.itemListView.SelectionChanged += ItemListView_SelectionChanged;

            // Start listening for Window size changes 
            // to change from showing two panes to showing a single pane
            Window.Current.SizeChanged += Window_SizeChanged;
      
            this.InvalidateVisualState();
             try
            {
                networkStatusCallback = new NetworkStatusChangedEventHandler(OnNetworkStatusChange);
                if (!registeredNetworkStatusNotification)
                {
                    NetworkInformation.NetworkStatusChanged += networkStatusCallback;
                    registeredNetworkStatusNotification = true;
                }
            }
            catch (Exception ex)
            {
                
            }
            this.InvalidateVisualState();
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            // Check for internet
            if (PopupInputContent.IsInternetAvailable())
            {
                //  Save any offline work orders
                SaveWorkOrdersToServer();
                if (Token.isFirstTime.Equals("true"))
                {
                    updateView();
                    this.InvalidateVisualState();
                    //RefreshList();
                    
               }
               else
               {
                   Token.isFirstTime = "true";
                   RefreshList();
               }
            }
            else
            {
                itemListView.ItemsSource = WorkOrderList.WorkOrdersList;
            }

               
        }
         async void OnNetworkStatusChange(object sender)
        {
            try
            {
                // get the ConnectionProfile that is currently used to connect to the Internet                
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

                if (InternetConnectionProfile != null)
                {
                    // we are connected to network
                    await SaveWorkOrdersToServer();
                    
                }
                InternetConnectionProfile = null;
            }
            catch (Exception ex)
            {
                
            }
        }
        // Register the Drag Event
        public delegate void DragItemsStartingEventHandler(object sender, DragItemsStartingEventArgs e);
        
        // To perform Refesh action while dragging the list
        private async void itemListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
             bool isNetworkAvailable= PopupInputContent.IsInternetAvailable();
            if (isNetworkAvailable)
            {
                ProgressRing1.IsActive = true;
                updateView();
                ProgressRing1.IsActive = false;
            }
            else
            {
                if (WorkOrderList.WorkOrdersList.Count > 0)
                    this.itemListView.ItemsSource = WorkOrderList.WorkOrdersList;
                else
                {
                    this.itemListView.ItemsSource = null;
                    MessageDialog dialog = new MessageDialog("Network is not Available!! Please try after sometime", "");
                    //await dialog.ShowAsync();
                }
            }
        }

        // push work orders from Local DB to Server
         public async Task SaveWorkOrdersToServer()
        {
            try
            {
                if (PopupInputContent.IsInternetAvailable())
                {
                    // If connected to internet, check for any WO in local Db and push them to server
                    WorkOrderViewModel wo = new WorkOrderViewModel();
                    List<WorkOrder> WorkOrdersToStore = wo.GetAllWorkOrders();
                
                    if (null != WorkOrdersToStore && WorkOrdersToStore.Count > 0)
                    {
                        foreach (var workorder in WorkOrdersToStore)
                        {
                            await UpdateWorkOrderStatus(workorder.Id, workorder.Work_Status__c, workorder.Suspend_Reason__c,workorder.WorkOrder_End_Date__c);
                            if (workorder.Comments__c != null)
                            {
                                await submitComments(workorder.Comments__c);
                            }
                            // Delete the saved Workorders from the Local DB
                            wo.DeleteWorkOrder(workorder.Id);
                        }
                        //updateView();
                        //RefreshList();
                    }
                }
            }
            catch (Exception ex)
            {

            }




        }


        // HTTP request method calling REST service to update the workorder status
         public async Task<String> UpdateWorkOrderStatus(string id,string Work_status, string SuspendedReason,string EndDate )
        {
            
            String wListURL = Token.InstanceUrl + "/services/apexrest/UpdateWorkOrdersStatus/" + id + "&" + Work_status + "&" + SuspendedReason + "&" + EndDate;


            HttpClient client = new HttpClient();
            HttpWebRequest request = HttpWebRequest.Create(wListURL) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";
            request.Headers["Authorization"] = "Bearer " + Token.AccessToken;
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            //var responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());

            // Pick up the response:
            string result = null;
            using (HttpWebResponse resp = await request.GetResponseAsync()
                                          as HttpWebResponse)
            {
                StreamReader reader =
                    new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }
            return result;
        }

        
        #region Logical page navigation

        // The split page isdesigned so that when the Window does have enough space to show
        // both the list and the dteails, only one pane will be shown at at time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        private const int MinimumWidthForSupportingTwoPanes = 768;

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <returns>True if the window should show act as one logical page, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation()
        {
            return Window.Current.Bounds.Width < MinimumWidthForSupportingTwoPanes;
        }

        /// <summary>
        /// Invoked with the Window changes size
        /// </summary>
        /// <param name="sender">The current Window</param>
        /// <param name="e">Event data that describes the new size of the Window</param>
        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.InvalidateVisualState();
        }

        
        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        ///  By Tallapannei
        
        private  void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When user clicks any item in the List
                  MakeSelection();

        }

         public async void MakeSelection()
        {
           
            try
            {
                String success = "true";
            
                if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();
                Selected_ListIndex = itemListView.SelectedIndex;
                if (Selected_ListIndex != -1)
                {
                    
                    CurrentWorkOrder.CurretntWorkOrderModelObj = WorkOrderList.WorkOrdersList[Selected_ListIndex];
                    Token.IdNew = WorkOrderList.WorkOrdersList[Selected_ListIndex].Id;
                    if (!(Token.IdNew.Equals(Token.IdOld)))
                    {
                        GPS.IsTapEnabled = true;
                        Token.IdOld = Token.IdNew;
                    }
                    // Check for any In_Progress work orders before selected work order
                    for (int index = 0; index < Selected_ListIndex; index++)
                    {
                        if (WorkOrderList.WorkOrdersList[index].Work_Status__c.Equals(Constants.STATUS_INPROGRESS)
                            || WorkOrderList.WorkOrdersList[index].Work_Status__c.Equals(Constants.STATUS_NOTSTARTED))
                        {
                            //ShowNotPossibleMessage();
                            IAsyncOperation<IUICommand> asyncCommand = null;
                            MessageDialog dialog = new MessageDialog("Please complete the previous WorkOrders", "");
                            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>  dialog.ShowAsync());
                            
                            //await asyncCommand;
                            itemListView.SelectedItem = null;
                            success = "false";
                            break;
                        }
                    }
                    if (success.Equals("true"))
                    {
                        // on success change the work order status to In_Progress
                        AssignedByTextBlock.Text = CurrentWorkOrder.CurretntWorkOrderModelObj.Assigned_By__r.Name;
                        if (WorkOrderList.WorkOrdersList[Selected_ListIndex].Work_Status__c.Equals(Constants.STATUS_NOTSTARTED))
                        {

                            WorkOrderList.WorkOrdersList[Selected_ListIndex].Work_Status__c = Constants.STATUS_INPROGRESS;
                            CurrentWorkOrder.CurretntWorkOrderModelObj.Work_Status__c = Constants.STATUS_INPROGRESS;
                            RefreshList();
                           

                        }
                        
                        populateMainPane();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

         // Fill the Dynamic work order data
        public void populateMainPane()
        {
            CustomerName.Text = CurrentWorkOrder.CurretntWorkOrderModelObj.Customer_Name__c;
            AddressText.Text = CurrentWorkOrder.CurretntWorkOrderModelObj.Address__c;
            CityText.Text = CurrentWorkOrder.CurretntWorkOrderModelObj.City__c;
            ZipText.Text = CurrentWorkOrder.CurretntWorkOrderModelObj.Zip_Code__c;
            ProductNameText.Text = CurrentWorkOrder.CurretntWorkOrderModelObj.Product_Name__c;
            ProductDetailsText.Text = CurrentWorkOrder.CurretntWorkOrderModelObj.Product_Details__c;

        }

        // Refresh the listView with updated work order list
        public void RefreshList()
        {
            this.itemListView.ItemsSource = null;
            this.itemListView.ItemsSource = WorkOrderList.WorkOrdersList;
            //this.Frame.Navigate(typeof(SplitPage1));
          
           
        }
        

        private void CommandInvokedHandler(IUICommand command)
        {

            switch (command.Label)
            {
                case "Yes":
                    WorkOrderList.WorkOrdersList[Selected_ListIndex].Work_Status__c = Constants.STATUS_INPROGRESS;
                    CurrentWorkOrder.CurretntWorkOrderModelObj.Work_Status__c = Constants.STATUS_INPROGRESS;
                    RefreshList();
                    break;
                case "No":
                    break;

            }
            
        }

       // In built methods to reload the whole page 
        private void InvalidateVisualState()
        {
            var visualState = DetermineVisualState();
            VisualStateManager.GoToState(this, visualState, false);
            
       }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        private string DetermineVisualState()
        {
            if (!UsingLogicalPageNavigation())
                return "PrimaryView";

            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation() && this.itemListView.SelectedItem != null;

            return logicalPageBack ? "SinglePane_Detail" : "SinglePane";
        }

        #endregion

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        /// Commented to delete NavagationHelper
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        //    navigationHelper.OnNavigatedFrom(e);
            DataTransferManager DT = DataTransferManager.GetForCurrentView();
            DT.DataRequested += new TypedEventHandler<DataTransferManager,
                    DataRequestedEventArgs>(this.ShareTextHandler);

        }

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;

            request.Data.Properties.Title = "Share Image";
            List<IStorageItem> imageItems = new List<IStorageItem>();
            imageItems.Add(Token.shareImage);
            request.Data.SetStorageItems(imageItems);
            
        }
        #endregion
        // used to update the worder order by dragging the itemListView
        private void itemListView_DragOver(object sender, DragEventArgs e)
        {
            ProgressRing1.IsActive = true;
            updateView();
        }

        private void itemDetail_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        
        
       
        // to share mail
        private void commandHandlerSharing(IUICommand command)
        {
            DataTransferManager.ShowShareUI();
        }

      

      
        // navigate to maps
       private void Image_Maps_Tapped(object sender, TappedRoutedEventArgs e)
       {
           // Maps Need to ne integrated
           this.Frame.Navigate(typeof(GetDirections));
       }

        /********************* image post changes begin*************************************************/

              
        // Get the Image stream and save to App private storage
       async void GetRequestStreamCallback(IAsyncResult callbackResult)
       {
           HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
           Stream postStream = myRequest.EndGetRequestStream(callbackResult);


           StorageFolder storageFolder = KnownFolders.PicturesLibrary;
           String _SignName = CurrentWorkOrder.CurretntWorkOrderModelObj.Id+"_"+"Sign"+".png";;
           StorageFile file = await storageFolder.TryGetItemAsync("Untitled.png") as StorageFile;
           if (file != null)
           {
               var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
               Byte[] bytesdata = buffer.ToArray();
               postStream.Write(bytesdata, 0, bytesdata.Length);
               myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
           }



       }

       void GetResponsetStreamCallback(IAsyncResult callbackResult)
       {

           try
           {
               HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
               HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult);
               string responseString = "";
               Stream streamResponse = response.GetResponseStream();
               StreamReader reader = new StreamReader(streamResponse);
               responseString = reader.ReadToEnd();

               string result = responseString;
           }
           catch (Exception ex)
           {

           }
       }
        /****************image post changes end************************************************************/
       private  void Image_Camera_Tapped(object sender, TappedRoutedEventArgs e)
       {
           UploadPhoto mUploadPhoto = new UploadPhoto();
           mUploadPhoto.PhotoCapture();
           
       }

       private void Image_Comments_Tapped(object sender, TappedRoutedEventArgs e)
       {
           // Comments Module Need to ne integrated
       }

       private void Image_Done_Tapped(object sender, TappedRoutedEventArgs e)
       {
           // Open the Completion popup
           if (!StandardPopup.IsOpen) { StandardPopup.IsOpen = true; }
           
           itemListView.ItemsSource = WorkOrderList.WorkOrdersList;
          
          
       }
     
       // used to get the HTML string, problem options from the database
       private void buttonWithFlyout1_Click(object sender, RoutedEventArgs e)
       {
           string webString = WorkOrderList.WorkOrdersList[0].Webpage__r.HTML__c;
           string problemOptions = WorkOrderList.WorkOrdersList[0].ProblemOptions__c;
           string[] semicolon = new string[] { ";" };
           String[] test = problemOptions.Split(semicolon, StringSplitOptions.None);
           webString = webString.Replace("Problem3", test[0]);
           webString = webString.Replace("Problem2", test[1]);
           webview2.NavigateToString(webString);
       }

       private async void submit_Click(object sender, RoutedEventArgs e)
       {
           
            string st1;
           // Invoke the javascript function called 'submit' that is loaded into the WebView.
           try
           {
               st1 = await webview2.InvokeScriptAsync("submit", null);
               htmlString = st1;
               if (htmlString.Contains("Identifier"))
               {
                   //string s = htmlString;
                   ErrorMessage.Text = "Please fill in the required fields marked with *";
               }
               else
               {
                   if (PopupInputContent.IsInternetAvailable())
                   {
                       await submitComments(htmlString);  

                   }
                   else
                   {
                       // storing comments in Local DB
                       using (var db = new SQLite.SQLiteConnection(App.DBPath))
                       {

                           WorkOrderViewModel workorder = new WorkOrderViewModel();
                           workorder.Id = CurrentWorkOrder.CurretntWorkOrderModelObj.Id; 
                           workorder.Comments__c = htmlString;
                           workorder.SaveWorkOrder(workorder);
                           Submit.IsTapEnabled = false;
                           esig.IsTapEnabled = true;
                           buttonWithFlyout1.IsEnabled = false;
                           form.Hide();
                       }
                           
                   }
               }
             

           }
           catch (Exception)
           {
               
           }
          
       }
        // method used to submit comments received regarding a particluar work order to the the database
       public async Task submitComments(string htmlString)
       {
           string workOrderId = WorkOrderList.WorkOrdersList[0].Id + "&" + htmlString;
           string wListURL = Token.InstanceUrl + "/services/apexrest/UpdateHTMLForms/" + workOrderId;
           HttpClient client = new HttpClient();
           HttpWebRequest request = HttpWebRequest.Create(wListURL) as HttpWebRequest;
           request.Method = "GET";
           request.ContentType = "application/json;charset=UTF-8";
           request.Headers["Authorization"] = "Bearer " + Token.AccessToken;
           HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
           StreamReader streamReader = new StreamReader(response.GetResponseStream());

           String st2 = streamReader.ReadToEnd();
           Submit.IsTapEnabled = false;
           
           esig.IsTapEnabled = true;
           buttonWithFlyout1.IsEnabled = false;
           form.Hide();
          
       }

       private void flyout_Opened(object sender, object e)
       {
           
            Flyout f = sender as Flyout;
            if (f != null)
            {
                
            }
        
       }
       // opens help when tapped
       private void help_Tapped(object sender, TappedRoutedEventArgs e)
       {
           this.Frame.Navigate(typeof(Help));
       }
        // opens esignature popup when tapped
       private void esig_Tapped(object sender, TappedRoutedEventArgs e)
       {
           esignature.IsOpen = true;
           done.IsTapEnabled = true;

       }

       private void RefresTapped(object sender, TappedRoutedEventArgs e)
       {
           
           // check whether internet is available or not and update the list.
           bool isNetworkAvailable = PopupInputContent.IsInternetAvailable();
           if (isNetworkAvailable)
           {
               ProgressRing1.IsActive = true;
               updateView();
               ProgressRing1.IsActive = false;
           }
           else
           {
               if (WorkOrderList.WorkOrdersList.Count > 0)
               {
                   this.itemListView.ItemsSource = null;
                   this.itemListView.ItemsSource = WorkOrderList.WorkOrdersList;
               }
               else
               {
                   this.itemListView.ItemsSource = null;
                   MessageDialog dialog = new MessageDialog("Network is not Available!! Please try after sometime", "");
                   //await dialog.ShowAsync();
               }
           }

       }

      
      
        

    }
}
