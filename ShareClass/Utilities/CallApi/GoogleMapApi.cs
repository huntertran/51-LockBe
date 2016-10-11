using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Newtonsoft.Json.Linq;
using ShareClass.Model.GoogleMap;

namespace ShareClass.Utilities.CallApi
{
    /// Google MAP Geocoding API Key: AIzaSyArRsbiDizXOVAHPI-Lj204ae8FsXgDkLA
    public class GoogleMapApi
    {
        //https://maps.googleapis.com/maps/api/geocode/json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&key=AIzaSyArRsbiDizXOVAHPI-Lj204ae8FsXgDkLA
        string key = "&key=AIzaSyArRsbiDizXOVAHPI-Lj204ae8FsXgDkLA";

        public async Task<Geocode> GetGpsFromAddressTask(string address)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + address + key;
            string json = await HttpService.SendAsync(url);
            if (string.IsNullOrEmpty(json)) return null;
            JObject jObject = JObject.Parse(json);
            return jObject.ToObject<Geocode>();
        }

        public async Task<Geocode> GetAddressFromGpsTask(BasicGeoposition b)
        {
            //https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + b.Latitude + "," + b.Longitude;
            string json = await HttpService.SendAsync(url);
            if (string.IsNullOrEmpty(json)) return null;
            JObject jObject = JObject.Parse(json);
            return jObject.ToObject<Geocode>();
        }
    }
}
