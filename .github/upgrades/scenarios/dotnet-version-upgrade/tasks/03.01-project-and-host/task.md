# 03.01-project-and-host: Update TFM to net8.0, replace ASP.NET packages, create Program.cs

## Objective
Update Tips.csproj TargetFramework from net48 to net8.0. Replace legacy ASP.NET Framework NuGet packages with the ASP.NET Core framework SDK. Create a minimal Program.cs with WebApplication host and a /health stub endpoint. Verify the app starts.

## Scope
- Tips\Tips.csproj — change TargetFramework, remove System.Web references, remove ASP.NET Framework NuGet packages, add Microsoft.AspNetCore.Mvc
- Tips\Program.cs — create new (does not yet exist)
- Tips\Global.asax / Global.asax.cs — remove once startup is moved to Program.cs

## Steps
1. In Tips.csproj: set `<TargetFramework>net8.0</TargetFramework>`
2. Remove all `<Reference>` items for System.Web.* (these are now framework-included in ASP.NET Core)
3. Remove Microsoft.AspNet.Mvc, Microsoft.AspNet.Razor, Microsoft.AspNet.WebPages, Microsoft.Web.Infrastructure NuGet packages
4. Create Program.cs with minimal WebApplication builder: AddControllersWithViews, MapControllerRoute default route, app.MapGet("/health", () => "ok")
5. Remove Global.asax and Global.asax.cs
6. Build and fix compilation errors in project file / startup only

**Done when**: Tips.csproj targets net8.0, Program.cs exists with minimal host, Global.asax removed, `dotnet build` compiles the project file without TFM-related errors (controller/model errors are expected and addressed in subsequent subtasks).
