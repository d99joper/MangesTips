# 03.01-project-and-host: Progress Details

## What Changed

### Tips\Tips.csproj
- Changed `TargetFramework` from `net48` to `net8.0`
- Removed `OutputType=Library` (not needed for ASP.NET Core web apps)
- Removed all `System.Web.*` framework `<Reference>` items
- Removed `Microsoft.AspNet.Mvc`, `Microsoft.AspNet.Razor`, `Microsoft.AspNet.WebPages`, `Microsoft.Web.Infrastructure` NuGet packages
- Updated `EntityFramework` to 6.5.2 (latest minor patch)
- Added `Microsoft.AspNetCore.Authentication.Cookies` package reference
- Removed `Global.asax` and `Global.asax.cs` Content/Compile entries
- Removed `AjaxControlToolkit` reference (System.Web-dependent, not compatible with net8.0)

### Tips\Program.cs (created)
- Minimal WebApplication host with AddControllersWithViews
- Cookie authentication registered (migrated from Forms Authentication — loginPath, timeout, cookie name preserved)
- UseAuthentication + UseAuthorization middleware
- Default MVC route: `{controller=Home}/{action=Index}/{id?}`
- /health stub endpoint

### Tips\Global.asax + Tips\Global.asax.cs (removed)
- Application_Start logic (AreaRegistration, RouteConfig, ScriptManager mapping) moved to Program.cs

## Build Result

`dotnet build` produces errors only in C# source files (controllers/helpers) that still reference `System.Web.Mvc` — these are addressed in subtasks 03.02–03.05. No project-file or MSBuild errors.

## Notes

- `AjaxControlToolkit` is a .NET Framework–only library. It is removed from references. Any views using AjaxControlToolkit controls will need manual cleanup.
