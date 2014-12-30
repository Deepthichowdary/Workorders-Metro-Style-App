using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WorkOrdersApp.Models
{
    [DataContract]
    public class MapData
    {
        [DataMember(Name = "copyright")]
        public string Copyright { get; set; }
        [DataMember(Name = "brandLogoUri")]
        public string BrandLogoUri { get; set; }
        [DataMember(Name = "statusCode")]
        public int StatusCode { get; set; }
        [DataMember(Name = "statusDescription")]
        public string StatusDescription { get; set; }
        [DataMember(Name = "authenticationResultCode")]
        public string AuthenticationResultCode { get; set; }
        [DataMember(Name = "errorDetails")]
        public string[] errorDetails { get; set; }
        [DataMember(Name = "traceId")]
        public string TraceId { get; set; }
        [DataMember(Name = "resourceSets")]
        public ResourceSet[] ResourceSets { get; set; }
    }

    [DataContract]
    public class ResourceSet
    {
        [DataMember(Name = "estimatedTotal")]

        public long EstimatedTotal { get; set; }
        [DataMember(Name = "resources")]
        public Location[] Resources { get; set; }
    }

    [DataContract]
    public class Point
    {
        
        [DataMember(Name = "coordinates")]
        public double[] Coordinates { get; set; }
    }


    [DataContract]
    public class BoundingBox
    {
        [DataMember(Name = "southLatitude")]
        public double SouthLatitude { get; set; }
        [DataMember(Name = "westLongitude")]
        public double WestLongitude { get; set; }
        [DataMember(Name = "northLatitude")]
        public double NorthLatitude { get; set; }
        [DataMember(Name = "eastLongitude")]
        public double EastLongitude { get; set; }
    }

    [DataContract]
    public class GeocodePoint : Point
    {
        [DataMember(Name = "calculationMethod")]
        public string CalculationMethod { get; set; }
        [DataMember(Name = "usageTypes")]
        public string[] UsageTypes { get; set; }
    }

    [DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Location
    {
        [DataMember(Name = "boundingBox")]
        public BoundingBox BoundingBox { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "point")]
        public Point Point { get; set; }
        [DataMember(Name = "entityType")]
        public string EntityType { get; set; }
        [DataMember(Name = "address")]
        public Address Address { get; set; }
        [DataMember(Name = "confidence")]
        public string Confidence { get; set; }
        [DataMember(Name = "geocodePoints")]
        public GeocodePoint[] GeocodePoints { get; set; }
        [DataMember(Name = "matchCodes")]
        public string[] MatchCodes { get; set; }
    }

    [DataContract]
    public class Address
    {
        [DataMember(Name = "addressLine")]
        public string AddressLine { get; set; }
        [DataMember(Name = "adminDistrict")]
        public string AdminDistrict { get; set; }
        [DataMember(Name = "adminDistrict2")]
        public string AdminDistrict2 { get; set; }
        [DataMember(Name = "countryRegion")]
        public string CountryRegion { get; set; }
        [DataMember(Name = "formattedAddress")]
        public string FormattedAddress { get; set; }
        [DataMember(Name = "locality")]
        public string Locality { get; set; }
        [DataMember(Name = "postalCode")]
        public string PostalCode { get; set; }
    }
}
