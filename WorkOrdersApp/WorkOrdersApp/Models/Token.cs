using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WorkOrdersApp.Models
{
    // Token is a static class that stores the access content for furthur Db access
        public static class Token
        {
            public static string AccessToken { get; set; }
            public static string RefreshToken { get; set; }
            public static string InstanceUrl { get; set; }
            public static string Id { get; set; }
            public static bool checkIn { get; set; }
            public static bool firstTimeLogin { get; set; }
            public static StorageFile shareImage { get; set; }
            public static String isFirstTime = "true";
            public static string IdNew {get; set;}
            public static string IdOld { get; set; }
        }

        
}
