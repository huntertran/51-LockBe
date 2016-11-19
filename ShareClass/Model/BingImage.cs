// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using ShareClass.ViewModel;

namespace ShareClass.Model
{
    public class PointOfInterest
    {
        public string desc { get; set; }
        public string link { get; set; }
        public string query { get; set; }
        public int locx { get; set; }
        public int locy { get; set; }
    }

    public class BingImage
    {
        public string startdate { get; set; }
        public string fullstartdate { get; set; }
        public string enddate { get; set; }
        public string url { get; set; }
        public string urlbase { get; set; }
        public string copyright { get; set; }
        public string copyrightlink { get; set; }
        public bool wp { get; set; }
        public string hsh { get; set; }
        public int drk { get; set; }
        public int top { get; set; }
        public int bot { get; set; }
        public List<PointOfInterest> hs { get; set; }
        public List<object> msg { get; set; }

        public string AppropriateLink { get; set; }
    }

    public class Tooltips
    {
        public string loading { get; set; }
        public string previous { get; set; }
        public string next { get; set; }
        public string walle { get; set; }
        public string walls { get; set; }
    }

    public class BingImageRoot
    {
        public List<BingImage> images { get; set; }
        public Tooltips tooltips { get; set; }
    }

    public class BingLanguage : ObservableObject
    {
        private string _code;
        private string _name;

        public string Code
        {
            get { return _code; }
            set
            {
                if (value == _code) return;
                _code = value;
                OnPropertyChanged();
            }
        }

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
    }
}
