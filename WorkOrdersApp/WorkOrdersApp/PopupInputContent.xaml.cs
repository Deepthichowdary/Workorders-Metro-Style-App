using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkOrdersApp.Models;
using WorkOrdersApp.ViewModels;
using System.Globalization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrdersApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopupInputContent : UserControl
    {
        String status;
        String SuspendedReason = "No";
        public PopupInputContent()
        {
            this.InitializeComponent();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
        private void CancelPopupClicked(object sender, RoutedEventArgs e)
        {
            Popup p = this.Parent as Popup;

            // close the Popup
            if (p != null) { p.IsOpen = false; }
        }
        private async void SubmitPopupClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // in this example we assume the parent of the UserControl is a Popup 
                Popup p = this.Parent as Popup;
                String id = CurrentWorkOrder.CurretntWorkOrderModelObj.Id;
                Boolean isValid = true;
                String _errorMsg = "";

                // Get today date in MM-DD-YYYY formate as Work order End date
                DateTime EndTime = new DateTime();
                CultureInfo cultureInfo = new CultureInfo("en-US");
                EndTime = DateTime.Now;
                String EndDate;
                EndDate = EndTime.ToString("d", cultureInfo);
                EndDate = EndDate.Replace('/', '-');

                //Get the selected status from combo box
                ComboBoxItem typeItem = (ComboBoxItem)StatusComboBox.SelectedItem;
                status = typeItem.Content.ToString();
                
                if (status.Equals("Suspended"))
                {
                    string reason = ReasonTextBox.Text;

                    //Get the Reason text from Textbox
                    if (!reason.Equals(""))
                    {
                        SuspendedReason = ReasonTextBox.Text;
                    }
                    else
                    {
                        _errorMsg = "Please enter the reason!!";
                        isValid = false;
                    }
                }
                else
                {
                    // If the status is Completed
                    SuspendedReason = "No";
                }

               
                if (isValid)
                {
                    
                    if (!IsInternetAvailable())
                    {
                        // In Offline mode
                        using (var db = new SQLite.SQLiteConnection(App.DBPath))
                        {
                            // Save the work order to local SQlite DB
                            WorkOrderViewModel workorder = new WorkOrderViewModel();
                            workorder.Suspend_Reason__c = SuspendedReason;
                            workorder.Work_Status__c = status;
                            workorder.Id = id;
                            workorder.WorkOrder_End_Date__c = EndDate;
                            workorder.SaveWorkOrder(workorder);

                            // Remove the WO from the list and refresh the screen
                            WorkOrderList.WorkOrdersList.RemoveAt(0);
                            List<WorkOrderModel> WM = new List<WorkOrderModel>();
                            WM = WorkOrderList.WorkOrdersList;
                            WorkOrderList.WorkOrdersList = null;
                            WorkOrderList.WorkOrdersList = WM;
                            CurrentWorkOrder.CurretntWorkOrderModelObj = null;
                            SplitPage1 mSplitPage1 = new SplitPage1();
                            mSplitPage1.RefreshList();
                            Frame rootFrame = Window.Current.Content as Frame;
                            //mSplitPage1.itemListView.ItemsSource = WorkOrderList.WorkOrdersList;
                            //this.rootFrame.Navigate(typeof(SplitPage1));
                            // close the Popup
                            if (p != null) { p.IsOpen = false; }
                           
                            
                        }
                    }
                    else
                    {
                        // If Network is available, push the work order to server and refresh the list
                        String result = await UpdateWorkOrderStatus(id, status, SuspendedReason,EndDate);
                        SplitPage1 mSplitPage1 = new SplitPage1();
                        mSplitPage1.updateView();
                        mSplitPage1.RefreshList();
                        // close the Popup
                        if (p != null) { p.IsOpen = false; }
                    }
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Please Enter a valid Reason!!", "");
                    await dialog.ShowAsync();

                }
            }
            catch (Exception ex)
            {

            }

        }

        //Check for Network Availability
        public static bool IsInternetAvailable()
        {

            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        // HTTP request method calling RESET service to update the workorder status
        public async Task<String> UpdateWorkOrderStatus(string id, string Work_status, string SuspendedReason,string endDate)
        {

            String wListURL = Token.InstanceUrl + "/services/apexrest/UpdateWorkOrdersStatus/" + id + "&" + Work_status + "&" + SuspendedReason + "&" + endDate;


            HttpClient client = new HttpClient();
            HttpWebRequest request = HttpWebRequest.Create(wListURL) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";
            request.Headers["Authorization"] = "Bearer " + Token.AccessToken;
            
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

        // On item selction from check box
        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Check if the status is suspended and enable the reason field
                ComboBoxItem typeItem = (ComboBoxItem)StatusComboBox.SelectedItem;
                string Selectedvalue = typeItem.Content.ToString();
                if (Selectedvalue.Equals("Suspended"))
                {
                    ReasonTextBox.IsEnabled = true;

                }
                else
                {
                    ReasonTextBox.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

    }
}
