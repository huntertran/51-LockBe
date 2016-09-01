using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShareClass.Model;
using ShareClass.Utilities;

namespace ShareClass.ViewModel.SettingGroup
{
    public class MoreAppViewModel : ObservableObject
    {
        private MoreAppsRootObject _appItems;
        private bool _isLoading;

        public MoreAppsRootObject AppItems
        {
            get { return _appItems; }
            set
            {
                if (Equals(value, _appItems)) return;
                _appItems = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (value == _isLoading) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public MoreAppViewModel()
        {
            GetMoreAppAsync();
        }

        public async void GetMoreAppAsync()
        {
            //Load more apps
            if (AppItems == null || AppItems.app.Count == 0)
            {
                AppItems = await GetMoreApps();
            }
        }

        public async Task<MoreAppsRootObject> GetMoreApps()
        {
            IsLoading = true;
            Debug.WriteLine("Get more apps");
            string result = await HttpService.SendAsync("https://sites.google.com/site/cuoilennaocacbanmoreapps/");
            string json = Regex.Split(result, "~~~")[1];
            
            IsLoading = false;

            var jObject = JObject.Parse(json);
            return jObject.ToObject<MoreAppsRootObject>();
        }
    }
}