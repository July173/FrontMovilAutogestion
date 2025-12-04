using System.Globalization;

namespace AutogestionSenaMaui.Converters;

public class BoolToChevronConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
        {
            return isExpanded ? "\uf282" : "\uf285"; // chevron-down : chevron-right
        }
        return "\uf285";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TabToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string selectedTab && parameter is string tabName)
        {
            return selectedTab == tabName ? Colors.White : Color.FromArgb("#E1E2ED");
        }
        return Color.FromArgb("#E1E2ED");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TabToTextColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string selectedTab && parameter is string tabName)
        {
            return selectedTab == tabName ? Color.FromArgb("#020817") : Color.FromArgb("#64748B");
        }
        return Color.FromArgb("#64748B");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
