using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Net;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
namespace WorkOrdersApp.Models
{
    class UploadPhoto
    {
        private WriteableBitmap wBitmap;
        Byte[] bytesdata, bytesdata1;
        
        public async void PhotoCapture()
        {

            DataTransferManager DT = DataTransferManager.GetForCurrentView();
            DT.DataRequested += new TypedEventHandler<DataTransferManager,
                    DataRequestedEventArgs>(this.ShareTextHandler);

            CameraCaptureUI cameraUI = new CameraCaptureUI();

            cameraUI.PhotoSettings.AllowCropping = false;
            cameraUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.MediumXga;

            // Open the camera in photo mode
            Windows.Storage.StorageFile capturedMedia =
                await cameraUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (capturedMedia != null)
            {
                using (var streamCamera = await capturedMedia.OpenAsync(FileAccessMode.Read))
                {
                    // Get the image stream

                    BitmapImage bitmapCamera = new BitmapImage();
                    bitmapCamera.SetSource(streamCamera);
                   
                    int width = bitmapCamera.PixelWidth;
                    int height = bitmapCamera.PixelHeight;

                    wBitmap = new WriteableBitmap(width, height);

                    using (var stream = await capturedMedia.OpenAsync(FileAccessMode.Read))
                    {
                        wBitmap.SetSource(stream);
                    }
                }
                 SaveImageAsJpeg();
            }
        }

        private async  void SaveImageAsJpeg()
        {
            
            // Create the File Picker control
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPG File", new List<string>() { ".jpg" });
            StorageFile file = await picker.PickSaveFileAsync();

            // Changes for share media
            if (file != null)
            {
                Token.shareImage = file;

                var messageDialog = new MessageDialog("Would you like to share the image?");
                messageDialog.Commands.Add(new UICommand(
           "yes",
           new UICommandInvokedHandler(this.commandHandlerSharing)));
                messageDialog.Commands.Add(new UICommand(
                    "cancel",
                    new UICommandInvokedHandler(this.commandHandlerSharing)));
                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;
                await messageDialog.ShowAsync();
               


                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    // Encode the image into JPG format,reading for saving
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    Stream pixelStream = wBitmap.PixelBuffer.AsStream();
                   // bytesdata = new byte[pixelStream.Length].ToArray();
                    Byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)wBitmap.PixelWidth, (uint)wBitmap.PixelHeight, 96.0, 96.0, pixels);
                   
                   
                    await encoder.FlushAsync();
                   
                }
                var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
                bytesdata1 = buffer.ToArray();
                await postimage(file.Name);
                
            }
            
        }

        private void commandHandlerSharing(IUICommand command)
        {
            if (command.Label.Equals("yes"))
            {
                DataTransferManager.ShowShareUI();
            }
        }

        // Post the image to the server
        public async Task postimage(string filename)
        {
            string workOrderId = WorkOrderList.WorkOrdersList[0].Id;
            string wListURL = Token.InstanceUrl + "/services/apexrest/PostImageData/" + workOrderId + "&" + filename;
            HttpWebRequest request = HttpWebRequest.Create(wListURL) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "image/jpeg";
            request.Headers["Authorization"] = "Bearer " + Token.AccessToken;
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);

        }

        async void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);


            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
                postStream.Write(bytesdata1, 0, bytesdata1.Length);
                myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
          



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

        
       
        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;

            request.Data.Properties.Title = "Share Image";
            List<IStorageItem> imageItems = new List<IStorageItem>();
            imageItems.Add(Token.shareImage);
            request.Data.SetStorageItems(imageItems);

        }
    }

}
