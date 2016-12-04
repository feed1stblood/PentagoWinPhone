using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Pentago.Helper
{
    public class PlayerToIconConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var player = (int)value;
            switch (player)
            {
                case 0:
                return new BitmapImage(new Uri("/Pentago;component/resource/human.png", UriKind.Relative));
                case 1:
                return new BitmapImage(new Uri("/Pentago;component/resource/computer1.png", UriKind.Relative));
                case 2:
                return new BitmapImage(new Uri("/Pentago;component/resource/computer2.png", UriKind.Relative));
                case 3:
                return new BitmapImage(new Uri("/Pentago;component/resource/computer3.png", UriKind.Relative));
                case 4:
                return new BitmapImage(new Uri("/Pentago;component/resource/computer4.png", UriKind.Relative));
                case 5:
                return new BitmapImage(new Uri("/Pentago;component/resource/computer5.png", UriKind.Relative));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
