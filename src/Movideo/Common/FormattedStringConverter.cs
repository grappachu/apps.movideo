using System;
using System.Globalization;
using System.Windows.Data;

namespace Grappachu.Apps.Movideo.Common
{
    [ValueConversion(typeof(object), typeof(string))]
    public class FormattedStringConverter : IValueConverter
    {
        /// <summary>
        ///     Attiva una nuova istanza di <see cref="FormattedStringConverter" />
        /// </summary>
        public FormattedStringConverter()
        {
            FormatString = "{0}";
        }

        /// <summary>
        ///     Ottiene o imposta la stringa di formattazione da applicare al valore
        /// </summary>
        public string FormatString { get; set; }



        /// <inheritdoc />
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null ? string.Format(FormatString, value) : string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

