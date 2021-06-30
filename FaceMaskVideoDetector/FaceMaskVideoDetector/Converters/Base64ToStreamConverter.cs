using System;
using System.IO;
using System.Globalization;
using Xamarin.Forms;

namespace FaceMaskVideoDetector.Converters
{
    public class Base64ToStreamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var byteArray = System.Convert.FromBase64String(value.ToString());
            var stream = new MemoryStream(byteArray);
            return ImageSource.FromStream(() => stream);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
