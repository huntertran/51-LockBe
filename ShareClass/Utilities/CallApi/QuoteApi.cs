using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShareClass.Model.Quote;

namespace ShareClass.Utilities.CallApi
{
    public class QuoteApi
    {
        public async Task<Quote> GetQuoteOfDay()
        {
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
            return new Quote {QuoteString = "Without music, life would be a mistake.", Author = "Friedrich Nietzsche" };
        }
    }
}
