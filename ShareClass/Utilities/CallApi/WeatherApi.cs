using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Newtonsoft.Json.Linq;
using ShareClass.Model.Weather;
using ShareClass.ViewModel.WeatherGroup;

namespace ShareClass.Utilities.CallApi
{
    ///Weather API
    /// Open weather API
    /// Link: http://openweathermap.org/current
    /// Limitation:
    /// max 60 call per min
    /// max 50 000 call per day
    /// Sample call: http://api.openweathermap.org/data/2.5/weather?lat=35&lon=139&appid=2de143494c0b295cca9337e1e96b00e0
    /// appid: de0aeea45fdaa31957490a980097a791
    /// Google MAP Geocoding API Key: AIzaSyArRsbiDizXOVAHPI-Lj204ae8FsXgDkLA
    public class WeatherApi
    {
        public async Task<CitiWeather> GetCityWeather(BasicGeoposition pos, UnitFormat unit = UnitFormat.Celcius)
        {
            string url =
                "http://api.openweathermap.org/data/2.5/weather?lat=" + pos.Latitude + "&lon=" +
                pos.Longitude + "&appid=" +
                "de0aeea45fdaa31957490a980097a791";
            switch (unit)
            {
                case UnitFormat.Celcius:
                    url += "&units=metric";
                    break;
                case UnitFormat.Fahrenheit:
                    url += "&units=imperial";
                    break;
                case UnitFormat.Kevin:
                    break;
            }
            string json =
                await
                    HttpService.SendAsync(url);

            if (!string.IsNullOrEmpty(json))
            {
                JObject jObject = JObject.Parse(json);
                return jObject.ToObject<CitiWeather>();
            }
            return null;
        }
    }
}