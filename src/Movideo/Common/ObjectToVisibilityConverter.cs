using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Grappachu.Apps.Movideo.Common
{
    /// <summary>
    ///     Definisce un convertitore di <see cref="Visibility" /> personalizzabile che accetta in input un oggetto
    /// </summary>
    /// <example>
    /// Creare un convertitore personalizzato tra le risorse risorse
    ///
    ///    <UserControl.Resources>
    ///       <local:ObjectToVisibilityConverter x:Key="ObjectToHiddenConverter" ConcreteValue="Visible" NullValue="Hidden" />
    ///    </UserControl.Resources>
    ///
    /// Utilizzare il controllo definito nel binding
    ///
    ///   <Button Content="Click me!" Command="{Binding ClickMeCommand}"
    ///           Visibility="{Binding SelectedItemValue, Converter={StaticResource ObjectToHiddenConverter}}" />
    /// 
    /// </example>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ObjectToVisibilityConverter : IValueConverter
    {
        /// <summary>
        ///     Attiva una nuova istanza di <see cref="ObjectToVisibilityConverter" />
        /// </summary>
        public ObjectToVisibilityConverter()
        {
            ConcreteValue = Visibility.Visible;
            NullValue = Visibility.Hidden;
        }

        /// <summary>
        ///     Ottiene o imposta il valore di <see cref="Visibility" /> da associare quando l'oggetto non è null
        /// </summary>
        public Visibility ConcreteValue { get; set; }

        /// <summary>
        ///     Ottiene o imposta il valore di <see cref="Visibility" /> da associare quando l'oggetto è null
        /// </summary>
        public Visibility NullValue { get; set; }

        /// <inheritdoc />
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value == null ? NullValue : ConcreteValue;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, ConcreteValue))
                return default(object);
            if (Equals(value, NullValue))
                return null;
            return null;
        }
    }
}