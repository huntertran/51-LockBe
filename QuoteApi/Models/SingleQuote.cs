using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace QuoteApi.Models
{
    public class SingleQuote
    {
        public int SingleQuoteId { get; set; }
        public string Quote { get; set; }
        public string Self
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture,
               "api/quotes/{0}", SingleQuoteId);
            }
            set { }
        }
    }
}