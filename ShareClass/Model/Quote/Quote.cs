using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShareClass.Model.Qoute
{

    //public class Quote
    //{
    //    public int id { get; set; }
    //    public string quote { get; set; }
    //    public string author { get; set; }
    //    public string permalink { get; set; }
    //}

    public class Quote
    {
        public string quote { get; set; }
        public string author { get; set; }
        public string category { get; set; }
        public string cat { get; set; }
    }

}
