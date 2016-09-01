using ShareClass.ViewModel;

namespace ShareClass.Model.Weather
{
    public class WeatherInfo : ObservableObject
    {
        private string _address;
        private string _temp;
        private string _condition;
        private string _mainCondition;


        public string Address
        {
            get { return _address; }
            set
            {
                if (value == _address) return;
                _address = value;
                OnPropertyChanged();
            }
        }

        public string Temp
        {
            get { return _temp; }
            set
            {
                if (value == _temp) return;
                _temp = value;
                OnPropertyChanged();
            }
        }

        public string Condition
        {
            get { return _condition; }
            set
            {
                if (value == _condition) return;
                _condition = value;
                OnPropertyChanged();
            }
        }

        public string MainCondition
        {
            get { return _mainCondition; }
            set
            {
                if (value == _mainCondition) return;
                _mainCondition = value;
                OnPropertyChanged();
            }
        }
    }
}
