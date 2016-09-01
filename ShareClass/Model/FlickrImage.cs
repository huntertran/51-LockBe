using System.Collections.Generic;
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace ShareClass.Model.Flickr
{
    public class Photo
    {
        public string id { get; set; }
        public string owner { get; set; }
        public string secret { get; set; }
        public string server { get; set; }
        public int farm { get; set; }
        public string title { get; set; }
        public int ispublic { get; set; }
        public int isfriend { get; set; }
        public int isfamily { get; set; }
    }

    public class Photos
    {
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public string total { get; set; }
        public List<Photo> photo { get; set; }
    }

    public class FlickrRootObject
    {
        public Photos photos { get; set; }
        public string stat { get; set; }
    }

    public class Size
    {
        public string label { get; set; }
        public object width { get; set; }
        public object height { get; set; }
        public string source { get; set; }
        public string url { get; set; }
        public string media { get; set; }
    }

    public class Sizes
    {
        public int canblog { get; set; }
        public int canprint { get; set; }
        public int candownload { get; set; }
        public List<Size> size { get; set; }
    }

    public class SizeRootObject
    {
        public Sizes sizes { get; set; }
        public string stat { get; set; }
    }
}
