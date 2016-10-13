using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using ShareClass.Utilities.Helpers;

namespace ShareClass.Utilities
{
    public class HttpService
    {
        public static async Task<HttpResponseMessage> GetResponse(string url, Dictionary<string, string> headers = null, HttpMethod requestMethod = null,
            Dictionary<string, string> inputs = null, bool allowAutoRedirect = true)
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
                AutomaticDecompression = DecompressionMethods.GZip |
                                         DecompressionMethods.Deflate |
                                         DecompressionMethods.None,
                AllowAutoRedirect = allowAutoRedirect
            };


            var httpClient = new HttpClient(handler);

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

        public static async Task<string> SendAsync(string url, Dictionary<string, string> headers = null , HttpMethod requestMethod = null,
            Dictionary<string, string> inputs = null, bool allowAutoRedirect = true)
        {
            var responseMessage = await GetResponse(url,headers, requestMethod, inputs, allowAutoRedirect);
            try
            {
                if (responseMessage == null) return null;
                if (responseMessage.StatusCode == HttpStatusCode.OK || responseMessage.StatusCode == HttpStatusCode.Found)
                {
                    string result = await responseMessage.Content.ReadAsStringAsync();

                    //Debug.WriteLine("HttpService.SendAsync\nAPI: {0}\nResult: {1}", url, result);

                    return result;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

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

        public static async Task DownloadImage(string url)
        {
            StorageFolder backgroundFolder =
                await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync("Background",
                        CreationCollisionOption.OpenIfExists);
            
            Uri uri = new Uri(url);
            string fileName = url.Split('/').Last();

            if (!StorageHelper.IsFileExisted(backgroundFolder, fileName))
            {
                StorageFile file = await
                    StorageFile.CreateStreamedFileFromUriAsync(fileName, uri,
                        RandomAccessStreamReference.CreateFromUri(uri));

                var fileList = await backgroundFolder.GetFilesAsync();
                foreach (StorageFile storageFile in fileList)
                {
                    if (StorageHelper.IsFileExisted(backgroundFolder, storageFile.Name))
                    {
                        await storageFile.DeleteAsync(StorageDeleteOption.Default);
                    }
                }

                await file.CopyAsync(backgroundFolder);
            }
            else
            {
                var currentBackground = await backgroundFolder.GetFileAsync(fileName);

                var basicProperties = await currentBackground.GetBasicPropertiesAsync();

                if (basicProperties.Size > 0) return;

                StorageFile file = await
                   StorageFile.CreateStreamedFileFromUriAsync(fileName, uri,
                       RandomAccessStreamReference.CreateFromUri(uri));

                var fileList = await backgroundFolder.GetFilesAsync();
                foreach (StorageFile storageFile in fileList)
                {
                    if (StorageHelper.IsFileExisted(backgroundFolder, storageFile.Name))
                    {
                        await storageFile.DeleteAsync(StorageDeleteOption.Default);
                    }
                }

                await file.CopyAsync(backgroundFolder);

            }
        }
    }
}