
## [2026-06-01 14:11] 01-prerequisites

Verified .NET 8 SDK is installed and compatible. No global.json present. Environment is ready for the upgrade.


## [2026-06-01 14:20] 02-sdk-conversion

Converted Tips.csproj to SDK-style format (Microsoft.NET.Sdk.Web). Migrated packages.config to PackageReference. Added explicit System.Web framework references. Set OutputType=Library for .NET Framework MVC web app. Project builds cleanly on net48 with 0 errors and 0 warnings.


## [2026-06-01 14:26] 03.01-project-and-host

Updated Tips.csproj to net8.0. Removed System.Web references and legacy ASP.NET packages. Created Program.cs with WebApplication host, cookie auth, and default MVC route. Removed Global.asax. Remaining build errors are in source files — addressed by subsequent subtasks.


## [2026-06-01 14:28] 03.02-configuration

Created appsettings.json and appsettings.Development.json. Added System.Configuration.ConfigurationManager package for EF6 compatibility. Added EF6 connection string bridge in Program.cs. Migrated AuthController from ConfigurationManager + FormsAuthentication to IConfiguration + cookie auth (async SignInAsync/SignOutAsync).


## [2026-06-01 14:33] 03.03-controllers

Migrated all 12 MVC controllers from System.Web.Mvc to Microsoft.AspNetCore.Mvc. Replaced JavaScriptSerializer with System.Text.Json. Fixed TimeZone deprecation, HttpUtility, [ValidateInput], [AllowHtml]. Deleted RouteConfig.cs. Remaining errors only in ASMX/ASHX/SendEmail helpers (03.05).


## [2026-06-01 14:33] 03.04-auth

Auth migration already completed in 03.01/03.02/03.03. FormsAuthentication fully replaced with ASP.NET Core cookie auth. No System.Web.Security references remain.


## [2026-06-01 15:56] 03.05-helpers-and-cleanup

PdfGenerator already fixed (IWebHostEnvironment) in a previous step. ASMX/ASHX code-behind files were already removed. Fixed two lingering build errors in NyKupongController: replaced `new SettingsRepository()` call with injected `_settingsRepo`, and changed `_settingsRepo` in BaseController from `private` to `protected`. Solution now builds with 0 errors and 0 warnings.


## [2026-06-01 16:05] 04-validation

EF6 → EF Core 8 migration complete. Removed EntityFramework 6.5.2 package; added EF Core 8.x SqlServer, Proxies, and Design packages. Rewrote Tips.Context.cs with DbContextOptions constructor and EF Core fluent API (HasOne/IsRequired/OnDelete). Deleted Tips.edmx, Tips.Designer.cs, Tips1.cs, Tips.Context.Partial.cs. Fixed repositories (System.Data.Entity → Microsoft.EntityFrameworkCore, ExecuteSqlCommand → ExecuteSqlRaw, nested Include → ThenInclude, static methods → instance methods). Updated Program.cs to use AddDbContext with UseSqlServer + UseLazyLoadingProxies. Injected UserRepository into StatisticsController. Fixed BaseController._settingsRepo visibility and NyKupongController dependency usage. Final build: 0 errors, 0 warnings. All EF6 artifacts verified removed.

