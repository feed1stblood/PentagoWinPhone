using System.Windows.Data;

namespace PentagoWeb.Helper
{
    public class BoardGridWidthConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                double width = (double)value;
                int numberInARow = int.Parse(parameter.ToString());
                return (width) / numberInARow;
            }
            return 0;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
