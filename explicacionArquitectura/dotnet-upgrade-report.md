# .NET 9 Upgrade Report

## Summary

The **AutogestionSenaMaui** MAUI project was successfully upgraded from .NET 8 to .NET 9. The project now builds successfully for both Android and Windows platforms.

## Project target framework modifications

| Project name                     | Old Target Framework                              | New Target Framework                               | Commits         |
|:---------------------------------|:-------------------------------------------------:|:--------------------------------------------------:|:----------------|
| AutogestionSenaMaui.csproj       | net8.0-android;net8.0-windows10.0.19041.0         | net9.0-android;net9.0-windows10.0.19041.0          | 240cfdb2, b263b738 |

## NuGet Packages

| Package Name                           | Old Version            | New Version | Commit Id |
|:---------------------------------------|:----------------------:|:-----------:|:----------|
| LiveChartsCore.SkiaSharpView.Maui      | 2.0.0-rc4.5            | 2.0.0-rc4.5 | 240cfdb2  |
| Microsoft.Extensions.Logging.Debug     | 8.0.1                  | 9.0.0       | 240cfdb2  |
| Microsoft.Maui.Controls                | 8.0.100                | 9.0.50      | 240cfdb2  |
| Microsoft.Maui.Controls.Compatibility  | 8.0.100                | 9.0.50      | 240cfdb2  |
| SkiaSharp                              | 2.88.9-preview.2.2     | 3.116.1     | 240cfdb2  |
| SkiaSharp.Views.Maui.Controls          | 2.88.9-preview.2.2     | 3.116.1     | 240cfdb2  |
| SkiaSharp.Views.Maui.Core              | 2.88.9-preview.2.2     | 3.116.1     | 240cfdb2  |

## All commits

| Commit ID  | Description                                                                                           |
|:-----------|:------------------------------------------------------------------------------------------------------|
| d5c44f82   | Commit upgrade plan                                                                                   |
| 240cfdb2   | Project upgraded and builds successfully for both net9.0-android and net9.0-windows10.0.19041.0      |
| b263b738   | Update AutogestionSenaMaui.csproj platform version for Windows                                        |
| 97b68d88   | Fix Android MainActivity - remove unavailable using directives and parameters                         |
| 892d8db0   | Remove BOM from XAML files for consistency                                                            |
| 89412564   | Fix Android MainApplication - remove unavailable using directives for .NET 9.0                        |

## Project feature upgrades

### AutogestionSenaMaui.csproj

Here is what changed for the project during upgrade:

- Updated target frameworks from `net8.0-android` and `net8.0-windows10.0.19041.0` to `net9.0-android` and `net9.0-windows10.0.19041.0`
- Removed `CheckEolWorkloads` property (no longer needed with .NET 9)
- Updated all NuGet packages to .NET 9 compatible versions
- Fixed Android platform files (`MainActivity.cs` and `MainApplication.cs`) to work correctly with .NET 9 MAUI

## Notes

- The `Frame` control is now marked as obsolete in .NET 9 MAUI. Consider migrating to `Border` in a future update.
- There are several binding compilation warnings that can be addressed by adding `x:DataType` to XAML files for better performance.
- HarfBuzzSharp library shows warnings about 16KB page size requirement for Android 16.

## Next steps

- Consider replacing `Frame` with `Border` throughout the application to address obsolete warnings
- Add `x:DataType` to XAML files to enable compiled bindings for better performance
- Update HarfBuzzSharp when a new version with 16KB page size support becomes available
