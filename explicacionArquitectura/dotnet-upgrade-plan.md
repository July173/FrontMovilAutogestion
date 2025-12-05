# .NET 9 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 9 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 9 upgrade.
3. Upgrade AutogestionSenaMaui.csproj to .NET 9

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                           | Current Version | New Version | Description                    |
|:---------------------------------------|:---------------:|:-----------:|:-------------------------------|
| LiveChartsCore.SkiaSharpView.Maui      | 2.0.0-rc4.5     | 2.0.0-rc4.5 | Keep current version           |
| Microsoft.Extensions.Logging.Debug     | 8.0.1           | 9.0.0       | Recommended for .NET 9         |
| Microsoft.Maui.Controls                | 8.0.100         | 9.0.50      | Required for .NET 9 MAUI       |
| Microsoft.Maui.Controls.Compatibility  | 8.0.100         | 9.0.50      | Required for .NET 9 MAUI       |
| SkiaSharp                              | 2.88.9-preview.2.2 | 3.116.1  | Recommended for .NET 9         |
| SkiaSharp.Views.Maui.Controls          | 2.88.9-preview.2.2 | 3.116.1  | Recommended for .NET 9         |
| SkiaSharp.Views.Maui.Core              | 2.88.9-preview.2.2 | 3.116.1  | Recommended for .NET 9         |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### AutogestionSenaMaui.csproj modifications

Project properties changes:
- Target frameworks should be changed from `net8.0-android;net8.0-windows10.0.19041.0` to `net9.0-android;net9.0-windows10.0.19041.0`

NuGet packages changes:
- Microsoft.Extensions.Logging.Debug should be updated from `8.0.1` to `9.0.0` (*recommended for .NET 9*)
- Microsoft.Maui.Controls should be updated from `8.0.100` to `9.0.50` (*required for .NET 9 MAUI*)
- Microsoft.Maui.Controls.Compatibility should be updated from `8.0.100` to `9.0.50` (*required for .NET 9 MAUI*)
- SkiaSharp should be updated from `2.88.9-preview.2.2` to `3.116.1` (*recommended for .NET 9*)
- SkiaSharp.Views.Maui.Controls should be updated from `2.88.9-preview.2.2` to `3.116.1` (*recommended for .NET 9*)
- SkiaSharp.Views.Maui.Core should be updated from `2.88.9-preview.2.2` to `3.116.1` (*recommended for .NET 9*)

Other changes:
- Remove CheckEolWorkloads property (no longer needed with .NET 9)
