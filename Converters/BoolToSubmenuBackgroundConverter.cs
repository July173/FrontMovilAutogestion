using System.Globalization;

namespace AutogestionSenaMaui.Converters;

/// <summary>
/// Converter para cambiar el fondo del submenú cuando está seleccionado
/// </summary>
public class BoolToSubmenuBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            // Verde oscuro con 80% opacidad para submenú seleccionado
            return Color.FromRgba(46, 125, 50, 0.8);
        }
        
        // Transparente para submenú no seleccionado
        return Colors.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
