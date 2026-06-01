# 02-sdk-conversion: Progress Details

## What Changed

### Tips\Tips.csproj
- Converted from legacy old-style `<Project ToolsVersion="...">` format to `<Project Sdk="Microsoft.NET.Sdk.Web">`
- Migrated from `packages.config` to `PackageReference` — `packages.config` removed
- Added explicit `System.Web`, `System.Web.ApplicationServices`, `System.Web.Extensions`, `System.Web.Routing`, `System.Web.Services` framework references (these were implicit in the old-style format but must be explicit in SDK-style on net48)
- Removed `netstandard` reference (not applicable for net48)
- Removed `NETStandard.Library` NuGet package (not needed for net48)
- Removed duplicate `EntityFramework` PackageReference (conversion tool emitted it twice)
- Set `OutputType=Library` (ASP.NET MVC web apps on .NET Framework are deployed as class libraries hosted by IIS — not standalone executables)
- Removed erroneous `OutputType=Exe` set by conversion tool
- `packages.config` deleted from disk

### Globbing exclusions (unchanged, surfaced for awareness)
The conversion tool detected 3 files on disk that were NOT in the original project and excluded them from compilation:
- `Models\Tips1.cs`
- `Models\Tips.Designer.cs`
- `Models\BonusPoint.cs`

These were intentionally excluded in the original project. They remain excluded.

## Build Result

`dotnet build Tips\Tips.csproj` → **0 errors, 0 warnings** on `net48`

## Issues Resolved

1. Missing `System.Web` assembly references (CS0246 on `WebService`, `HttpContext`, `RouteCollection`, etc.) — fixed by adding explicit `<Reference>` items
2. `CS5001: Program does not contain a static 'Main' method` — fixed by setting `OutputType=Library`
3. Broken XML from empty `<PropertyGroup>` tag — fixed inline
4. Malformed `Label` attribute (missing closing quote) — fixed inline
