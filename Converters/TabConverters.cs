using System.Globalization;

namespace AutogestionSenaMaui.Converters;

public class BoolToChevronConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
        {
            // Font Awesome 7: chevron-down (\uf078) : chevron-right (\uf054)
            return isExpanded ? "\uf078" : "\uf054";
        }
        return "\uf054";
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
