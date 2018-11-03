using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Grappachu.Movideo.Core.Models;

namespace Grappachu.Apps.Movideo.Common
{
    [ValueConversion(typeof(IEnumerable<MovieGenere>), typeof(string))]
    public class GenreListToStringConverter : IValueConverter
    {
        /// <summary>
        ///     Attiva una nuova istanza di <see cref="ArrayToStringConverter" />
        /// </summary>
        public GenreListToStringConverter()
        {
            Delimiter = ", ";
        }

        /// <summary>
        ///     Ottiene o imposta la stringa di formattazione da applicare al valore
        /// </summary>
        public string Delimiter { get; set; }



        /// <inheritdoc />
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null ? string.Join(Delimiter, ((IEnumerable<MovieGenere>)value).Select(x => x.Name)) : string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}