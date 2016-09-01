using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShareClass.Model.GoogleMap
{
    public class Geocode
    {
        [JsonProperty("results")]
        public List<Result> Results { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
    public class Result
    {
        [JsonProperty("address_components")]
        public List<AddressComponents> AddressComponents { get; set; }
        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }
        [JsonProperty("types")]
        public List<string> Types { get; set; }
    }
    public class AddressComponents
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }
        [JsonProperty("short_name")]
        public string ShortName { get; set; }
        [JsonProperty("types")]
        public List<string> Types { get; set; }
    }
    public class Geometry
    {
        [JsonProperty("location")]
        public Location Location { get; set; }
        [JsonProperty("location_type")]
        public string LocationType { get; set; }
        [JsonProperty("viewport")]
        public Viewport Viewport { get; set; }
    }
    public class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
    public class Viewport
    {
        [JsonProperty("northeast")]
        public Northeast Northeast { get; set; }
        [JsonProperty("southwest")]
        public Southwest Southwest { get; set; }
    }
    public class Northeast
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
    public class Southwest
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

}
