using System.Collections.ObjectModel;
using ShareClass.ViewModel;

namespace ShareClass.Model.Rss
{
    public class RssItem : ObservableObject
    {
        private string _description;
        private string _title;
        private string _link;
        private string _time;

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return _link; }
            set
            {
                if (value == _link) return;
                _link = value;
                OnPropertyChanged();
            }
        }

        public string Time
        {
            get { return _time; }
            set
            {
                if (value == _time) return;
                _time = value;
                OnPropertyChanged();
            }
        }
    }

    public class RssChannel : ObservableObject
    {
        private string _title;
        private string _link;
        private string _description;
        private string _language;
        private ObservableCollection<RssItem> _itemList;

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return _link; }
            set
            {
                if (value == _link) return;
                _link = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Language
        {
            get { return _language; }
            set
            {
                if (value == _language) return;
                _language = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RssItem> ItemList
        {
            get { return _itemList; }
            set
            {
                if (Equals(value, _itemList)) return;
                _itemList = value;
                OnPropertyChanged();
            }
        }
    }
}
