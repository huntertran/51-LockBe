using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShareClass.Model.Qoute;

namespace ShareClass.Utilities.CallApi
{
    public class QuoteApi
    {
        public async Task<Quote> GetQuoteOfDay()
        {
            // Old Api
            //string url = "http://quotes.stormconsultancy.co.uk/random.json";
            //string json = await StaticMethod.GetHttpAsString(url);

            //New POST API Header
            //{"X-Mashape-Key", "8ptmFgWPNumsh2bDQ6E82XTnJvvkp1LUOtfjsn0VYqUDqCP9iO"},
            //{"Content-Type", "application/x-www-form-urlencoded"},
            //{"Accept", "application/json"},


            var uri = "https://andruxnet-random-famous-quotes.p.mashape.com/?cat=famous";

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"X-Mashape-Key", "8ptmFgWPNumsh2bDQ6E82XTnJvvkp1LUOtfjsn0VYqUDqCP9iO"},
                {"Accept", "application/json"},
                {"Content-Type", "application/x-www-form-urlencoded"}
            };

            var json = await HttpService.SendAsync(uri, headers, HttpMethod.Post);

            //var json = await StaticMethod.PostAsync(uri);
            if (!string.IsNullOrEmpty(json))
            {
                JObject jObject = JObject.Parse(json);
                return jObject.ToObject<Quote>();

            }
            return new Quote {quote = "Without music, life would be a mistake.", author = "Friedrich Nietzsche" };
        }
    }
}
