namespace AutogestionSenaMaui.Helpers;

public static class SideMenuLayoutHelper
{
    /// <summary>
    /// Calcula el ancho del side menu (en DPs) dado el ancho disponible del parent o el ancho de pantalla
    /// </summary>
    public static double ComputeWidth(double parentWidth, double screenWidthDp)
    {
        if (parentWidth > 0) return parentWidth * 0.5;
        if (screenWidthDp > 0) return screenWidthDp * 0.5;
        return 320; // fallback
    }

    /// <summary>
    /// Calcula el alto del side menu como el alto disponible del MainContent (entre TopBar y Footer)
    /// </summary>
    public static double ComputeHeight(double mainContentHeight)
    {
        if (mainContentHeight > 0) return mainContentHeight;
        return 400; // fallback
    }
}
