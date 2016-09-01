using ShareClass.ViewModel;

namespace ShareClass.Model
{
    public enum MenuFunc
    {
        Start,
        ImageSource,
        WeatherForecast,
        Calendar,
        Rss,
        Quote,
        Note,
        Personalize,
        Settings,
        About
    };

    public class MenuListItem : ObservableObject
    {

        private string _name;
        private string _icon;
        private MenuFunc _menuF;
        private bool _isEnabled;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Icon
        {
            get { return _icon; }
            set
            {
                if (value == _icon) return;
                _icon = value;
                OnPropertyChanged();
            }
        }

        public MenuFunc MenuF
        {
            get { return _menuF; }
            set
            {
                if (value == _menuF) return;
                _menuF = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value.Equals(_isEnabled)) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }
    }
}
