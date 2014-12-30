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
using Windows.Devices.Geolocation;
using WorkOrdersApp.Models;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrdersApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetDirections : Page
    {
       private Geolocator geolocator = null;
       public GetDirections()
        {
            this.InitializeComponent();

            geolocator = new Geolocator();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            GetDirection();


        }
        // goes back split page
        private  void back_Click(object sender, RoutedEventArgs e)

        {
            Token.checkIn = true;
            Token.isFirstTime = "Map";
            this.Frame.Navigate(typeof(SplitPage1));
        }

        // checks to ake sure whether the technician has reached client location
        private async void  Checkin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                string wListURL = "http://dev.virtualearth.net/REST/v1/Locations/US/" + WorkOrderList.WorkOrdersList[0].State__c + "/" + WorkOrderList.WorkOrdersList[0].City__c + "/" + WorkOrderList.WorkOrdersList[0].Address__c + "/" + "?&key=AoX8xNBWyF_af0I0zbybYzpckutQ6HxSmuzuUGMsZrvfsgm8CUbt_Xj2oRN7WL8U";

                HttpWebRequest request = HttpWebRequest.Create(wListURL) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/json;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                String st1 = streamReader.ReadToEnd();
                MapData mp = new MapData();
                mp = JsonConvert.DeserializeObject<MapData>(st1);
                Double[] locNum = mp.ResourceSets[0].Resources[0].Point.Coordinates;



                Geoposition pos = await geolocator.GetGeopositionAsync();
                double curLat = Math.Round(pos.Coordinate.Latitude, 0);
                double curLon = Math.Round(pos.Coordinate.Longitude, 0);
                double LocLat = Math.Round(locNum[0], 0);
                double LocLon = Math.Round(locNum[1], 0);
                if ((LocLat == curLat) && (LocLon == curLon))
                {
                    //checkin
                    Token.checkIn = true;
                    Token.isFirstTime = "Map";
                    this.Frame.Navigate(typeof(SplitPage1));
                }
               
            }
            catch (Exception ex)
            {
                String exception = ex.Message;
            }
            Token.isFirstTime = "Map";
        }

        // shows navigateion from current point to destination address.
        public async void GetDirection()
        {
            Bing.Maps.Directions.Waypoint startWaypoint = new Bing.Maps.Directions.Waypoint("UHCL, Houston,TX");
           
            try
            {
                
                 Geoposition pos = await geolocator.GetGeopositionAsync();
               Bing.Maps.Location lc =  new Bing.Maps.Location(pos.Coordinate.Latitude,pos.Coordinate.Longitude);
               Bing.Maps.Directions.Waypoint startWaypoint1 = new Bing.Maps.Directions.Waypoint(lc);
                 startWaypoint = startWaypoint1;
            }
            catch (Exception ex)
            {
                String exception = ex.Message;
            }

            string DestAddress = WorkOrderList.WorkOrdersList[0].Address__c + "," + WorkOrderList.WorkOrdersList[0].City__c + "," + WorkOrderList.WorkOrdersList[0].State__c;
            Bing.Maps.Directions.Waypoint endWaypoint = new Bing.Maps.Directions.Waypoint(DestAddress);

            Bing.Maps.Directions.WaypointCollection waypoints = new Bing.Maps.Directions.WaypointCollection();
            waypoints.Add(startWaypoint);
            waypoints.Add(endWaypoint);

            Bing.Maps.Directions.DirectionsManager directionsManager = myMap.DirectionsManager;
            directionsManager.Waypoints = waypoints;

            Bing.Maps.Directions.RouteResponse response = await directionsManager.CalculateDirectionsAsync();

            directionsManager.ShowRoutePath(response.Routes[0]);


        }
    }
}
