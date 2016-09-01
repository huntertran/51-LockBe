using ShareClass.ViewModel;

namespace ShareClass.Model
{
    public class ImageSourceItem : ObservableObject
    {
        private string _name;
        private int _number;

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

        public int Number
        {
            get { return _number; }
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }
    }
}
