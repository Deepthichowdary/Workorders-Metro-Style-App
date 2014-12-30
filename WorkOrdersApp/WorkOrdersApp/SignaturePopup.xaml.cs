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
using Windows.Devices.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Shapes;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.Security;
using WorkOrdersApp.Models;
using System.Net;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrdersApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignaturePopup : UserControl
    {
        Byte[] BYTESDATA;
        InkManager _inkManager = new Windows.UI.Input.Inking.InkManager();
        private uint _penID;
        private uint _touchID;
        private Windows.Foundation.Point _previousContactPt;
        private Windows.Foundation.Point currentContactPt;
        private double x1;
        private double y1;
        private double x2;
        private double y2;
         private WriteableBitmap wBitmap;
        public SignaturePopup()
        {
            this.InitializeComponent();
        }
        // Method called when user relases the pointer
        public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (e.Pointer.PointerId == _penID)
                {
                    Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(panelcanvas);

                    // Pass the pointer information to the InkManager. 
                    _inkManager.ProcessPointerUp(pt);
                }

                else if (e.Pointer.PointerId == _touchID)
                {
                    // Process touch input
                }

                _touchID = 0;
                _penID = 0;

                // Call an application-defined function to render the ink strokes.
                e.Handled = true;
            } 
            catch(Exception ex){
                e.Handled = true;
            }
        }

        // This method called recursively during signature
        private void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (e.Pointer.PointerId == _penID)
                {
                    PointerPoint pt = e.GetCurrentPoint(panelcanvas);

                    // Render a green line on the canvas as the pointer moves. 
                    currentContactPt = pt.Position;
                    x1 = _previousContactPt.X;
                    y1 = _previousContactPt.Y;
                    x2 = currentContactPt.X;
                    y2 = currentContactPt.Y;

                    if (Distance(x1, y1, x2, y2) > 2.0)
                    {
                        Line line = new Line()
                        {
                            X1 = x1,
                            Y1 = y1,
                            X2 = x2,
                            Y2 = y2,
                            StrokeThickness = 4.0,
                            Stroke = new SolidColorBrush(Colors.Blue)
                        };

                        _previousContactPt = currentContactPt;

                        // Draw the line on the canvas by adding the Line object as
                        // a child of the Canvas object.
                        panelcanvas.Children.Add(line);

                        // Pass the pointer information to the InkManager.
                        _inkManager.ProcessPointerUpdate(pt);
                    }



                }

                else if (e.Pointer.PointerId == _touchID)
                {
                    // Process touch input
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void InkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // Get information about the pointer location.
                PointerPoint pt = e.GetCurrentPoint(panelcanvas);
                _previousContactPt = pt.Position;

                // Accept input only from pen, touch or mouse with the left button pressed. 
                PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
                if (pointerDevType == PointerDeviceType.Pen || pointerDevType == PointerDeviceType.Touch ||
                        pointerDevType == PointerDeviceType.Mouse &&
                        pt.Properties.IsLeftButtonPressed)
                {
                    // Pass the pointer information to the InkManager.
                    _inkManager.ProcessPointerDown(pt);
                    _penID = pt.PointerId;

                    e.Handled = true;
                }

                else if (pointerDevType == PointerDeviceType.Touch)
                {
                    // Process touch input
                    // Need to display it is not supported by the e-signature
                }
            }
            catch (Exception ex)
            {

            }
        }


        // On clicking save button
        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsInternetAvailable())
                {
                    // checking for signature field is empty
                if (_inkManager.GetStrokes().Count > 0)
                {
                    // save the signatute field to the secure app storage
                    String _SignName = CurrentWorkOrder.CurretntWorkOrderModelObj.Id + "_" + "Sign" + ".jpeg";
                    StorageFile myMerge = await ApplicationData.Current.LocalFolder.CreateFileAsync(_SignName, CreationCollisionOption.ReplaceExisting);
                    IOutputStream ac = await myMerge.OpenAsync(FileAccessMode.ReadWrite);
                    if (ac != null)
                    {
                        await _inkManager.SaveAsync(ac);
                    }
                    this.esignature.Visibility = Visibility.Collapsed;
                    ac.Dispose();
                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(myMerge);

                    BYTESDATA = buffer.ToArray();
                    // upload the esignature to the server
                    await upload_Signature(_SignName);
                }
                else
                {
                    var dlge = new MessageDialog("Please sign here before saving");
                    dlge.ShowAsync();
                }
                }
                else
                {
                    // If network is not available. Save that to local storage

                    var dlge = new MessageDialog("Network is not available!! But Stored Locally");
                    String _SignName = CurrentWorkOrder.CurretntWorkOrderModelObj.Id + "_" + "Sign" + ".jpeg";
                    StorageFile myMerge = await ApplicationData.Current.LocalFolder.CreateFileAsync(_SignName, CreationCollisionOption.ReplaceExisting);
                    IOutputStream ac = await myMerge.OpenAsync(FileAccessMode.ReadWrite);
                    if (ac != null)
                    {
                        await _inkManager.SaveAsync(ac);
                    }
                    this.esignature.Visibility = Visibility.Collapsed;
                    dlge.ShowAsync();

                }
            }
            catch (Exception ex)
            {
               
                var dlge = new MessageDialog("Please sign here before saving");
            } 
             
        }

        //Check for Network Availability
        public static bool IsInternetAvailable()
        {

            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        // Method to upload signature to server
        public async Task upload_Signature(string filename)
        {
            string workOrderId = WorkOrderList.WorkOrdersList[0].Id;
            string wListURL = Token.InstanceUrl + "/services/apexrest/PostImageData/" + workOrderId + "&" + filename;
            HttpWebRequest request = HttpWebRequest.Create(wListURL) as HttpWebRequest;
            request.Method = "POST";
            // request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "image/jpeg";
            request.Headers["Authorization"] = "Bearer " + Token.AccessToken;
            //request.GetRequestStreamAsync
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
        }

        async void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);


            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            try
            {
                    postStream.Write(BYTESDATA, 0, BYTESDATA.Length);
                    myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
            }
            catch(Exception ex)
             {

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


        private void Panelcanvas_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            //throw new NotImplementedException();
        }


        // Erase the canvas
        private void BtnErase_OnClick(object sender, RoutedEventArgs e)
        {
 
            panelcanvas.Children.Clear();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            panelcanvas.Background = new SolidColorBrush(Colors.Gray);
            this.esignature.Visibility = Visibility.Collapsed;
            
        }

        // find the distance to draw the line in signature
        private double Distance(double x1, double y1, double x2, double y2)
        {
            try
            {
                double d = 0;
                d = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
                return d;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private void panelcanvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
