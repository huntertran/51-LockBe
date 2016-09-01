using System;
using Windows.UI.Xaml.Data;

namespace ShareClass.Utilities.Converters
{
    public class PageToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string defaultName = "Lock Be";

            //if (value is StartPage)
            //{
            //    return defaultName;
            //}

            return defaultName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
