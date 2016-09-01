using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Nito.AsyncEx;

namespace BingDetectResolution
{
    class Program
    {

        static readonly List<string> FullResults = new List<string>();
        private static readonly List<Resolution> ResList = new List<Resolution>();

        public static async Task<bool> GetHeadTask(string url)
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri(url, UriKind.Absolute)
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static async Task<HttpResponseMessage> GetResponse(string url, Dictionary<string, string> headers = null, HttpMethod requestMethod = null,
            Dictionary<string, string> inputs = null)
        {
            List<KeyValuePair<string, string>> contents = null;

            if (inputs != null)
            {
                contents = inputs.ToList();

                foreach (var content in contents)
                {
                    Debug.WriteLine(content.ToString());
                }
            }

            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };

            HttpClient httpClient = new HttpClient(handler);

            if (requestMethod == null)
            {
                requestMethod = HttpMethod.Get;
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = requestMethod,
                RequestUri = new Uri(url, UriKind.Absolute)
            };

            if (contents != null)
            {
                requestMessage.Content = new FormUrlEncodedContent(contents);
            }
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in headers)
                {
                    requestMessage.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
                    Debug.WriteLine("Headers: {0}:{1}", keyValuePair.Key, keyValuePair.Value);
                }
            }

            try
            {
                HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                return responseMessage;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<string> SendAsync(string url, Dictionary<string, string> headers = null, HttpMethod requestMethod = null,
            Dictionary<string, string> inputs = null)
        {
            var responseMessage = await GetResponse(url, headers, requestMethod, inputs);
            try
            {
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string result = await responseMessage.Content.ReadAsStringAsync();

                    Debug.WriteLine("HttpService.SendAsync\nAPI: {0}\nResult: {1}", url, result);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        static void Main()
        {
            //AsyncContext.Run(() => GetListResolution());
            //AsyncContext.Run(() => CheckList());
            AsyncContext.Run(() => DectectParallel(1, 3000));
            foreach (string s in FullResults)
            {
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }

        static async void DectectParallel(int start, int end)
        {
            await MainAsync(start, end);
        }

        static async Task MainAsync(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                for (int j = 1; j < 3000; j++)
                {
                    if (IsValid(i, j))
                    {
                        await Check(i, j);
                    }
                }
            }

            foreach (string s in FullResults)
            {
                Console.WriteLine(s);
            }
        }

        static bool IsValid(int first, int second)
        {
            //1 : 1
            //4 : 3
            //5 : 3
            //5 : 4
            //8 : 5
            //16 : 9
            bool result = false;
            int gdc = Gcd(first, second);

            int one = first/gdc;
            int two = second/gdc;

            switch (one)
            {
                case 1:
                    if (two == 1)
                    {
                        result = true;
                    }
                    break;
                case 4:
                    if (two == 3)
                    {
                        result = true;
                    }
                    break;
                case 5:
                    if (two == 3)
                    {
                        result = true;
                    }
                    if (two == 4)
                    {
                        result = true;
                    }
                    break;
                case 8:
                    if (two == 5)
                    {
                        result = true;
                    }
                    break;
                case 16:
                    if (two == 9)
                    {
                        result = true;
                    }
                    if (two == 10)
                    {
                        result = true;
                    }
                    break;
            }

            if (!result)
            {
                int temp = one;
                one = two;
                two = temp;

                switch (one)
                {
                    case 1:
                        if (two == 1)
                        {
                            result = true;
                        }
                        break;
                    case 4:
                        if (two == 3)
                        {
                            result = true;
                        }
                        break;
                    case 5:
                        if (two == 3)
                        {
                            result = true;
                        }
                        if (two == 4)
                        {
                            result = true;
                        }
                        break;
                    case 8:
                        if (two == 5)
                        {
                            result = true;
                        }
                        break;
                    case 16:
                        if (two == 9)
                        {
                            result = true;
                        }
                        if (two == 10)
                        {
                            result = true;
                        }
                        break;
                }
            }

            return result;
        }

        static async Task CheckList()
        {
            foreach (Resolution resolution in ResList)
            {
                await Check(resolution.Width, resolution.Height);
            }
        }

        static async Task Check(int width, int height)
        {
            string imageLink = "http://www.bing.com/az/hprichbg/rb/GFLions_EN-US11413405777_";
            string link = $"{imageLink}{width}x{height}.jpg";
            bool result = await GetHeadTask(link);
            Console.WriteLine($"Width: {width} | Height: {height} | Status: {result}");
            if (result)
            {
                FullResults.Add(link);
            }
        }

        static async Task GetListResolution()
        {
            string link = "https://en.wikipedia.org/wiki/List_of_common_resolutions";
            string html = await SendAsync(link);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var trs =
                doc.DocumentNode
                    .Descendants("table")
                    .First(d => d.Attributes.Contains("class") &&
                            d.Attributes["class"].Value.Contains("wikitable"))
                    .Descendants("tr");

            foreach (HtmlNode tr in trs)
            {
                Resolution r = new Resolution();
                int width;
                if (int.TryParse(tr.ChildNodes[3].InnerText, out width))
                {
                    r.Width = width;
                    r.Height = int.Parse(tr.ChildNodes[7].InnerText);
                    ResList.Add(r);
                }
            }
        }

        static int Gcd(int a, int b)
        {
            while (b > 0)
            {
                int rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }
    }

    public class Resolution
    {
        public int Width;
        public int Height;
    }
}
