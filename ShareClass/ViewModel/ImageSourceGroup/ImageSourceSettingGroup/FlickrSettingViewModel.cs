using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShareClass.Model.Flickr;
using ShareClass.Utilities;

namespace ShareClass.ViewModel.ImageSourceGroup.ImageSourceSettingGroup
{
    public class FlickrSettingViewModel : ObservableObject
    {
        FlickrRootObject _flickrImageRoot;

        public FlickrRootObject FlickrImageRoot
        {
            get { return _flickrImageRoot; }
            set
            {
                if (Equals(value, _flickrImageRoot)) return;
                _flickrImageRoot = value;
                OnPropertyChanged();
            }
        }

        public async Task GetImageRoot()
        {
            string link =
                "https://api.flickr.com/services/rest/?method=flickr.interestingness.getList&api_key=6d57b69c9918b7e414369e926f1ae393&per_page=5&format=json&nojsoncallback=1";

            string result = await HttpService.SendAsync(link);

            JObject j = JObject.Parse(result);
            FlickrImageRoot = j.ToObject<FlickrRootObject>();
        }
    }
}
