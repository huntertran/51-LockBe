using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShareClass.Model.Flickr;

namespace ShareClass.Utilities.Helpers.SourceDataHelper
{
    public class FlickrHelper
    {
        public async Task<string> GenerateImageLink(Photo p)
        {
            //https://farm{farm-id}.staticflickr.com/{server-id}/{id}_{secret}_[mstzb].jpg

            string link =
                "https://api.flickr.com/services/rest/?method=flickr.photos.getSizes&api_key=6d57b69c9918b7e414369e926f1ae393&photo_id=" +
                p.id + "&format=json&nojsoncallback=1";

            string result = await HttpService.SendAsync(link);

            JObject j = JObject.Parse(result);
            SizeRootObject rootObject = j.ToObject<SizeRootObject>();
            return rootObject.sizes.size.Last().source;
        }
    }
}
