# 03.05-helpers-and-cleanup: Fix Helpers (PdfGenerator HttpServerUtilityBase, ASMX/ASHX files) and final cleanup

## Objective
Fix remaining System.Web usages in Helpers, remove ASMX/ASHX web service files (not supported in ASP.NET Core), and clean up Web.config references.

## Scope
- Tips\Helpers\PdfGenerator.cs — uses `HttpServerUtilityBase server` parameter (1 usage in method signature, used for Server.MapPath)
- Tips\Helpers\TopScorer.asmx / TopScorer.asmx.cs — ASMX web service, not supported in ASP.NET Core
- Tips\Helpers\TopScorers.ashx / TopScorers.ashx.cs — ASHX handler, not supported in ASP.NET Core
- Tips\Web.config — legacy, keep for EF6 connection string fallback if needed

## Steps
1. PdfGenerator.cs: Replace `HttpServerUtilityBase server` parameter with `IWebHostEnvironment env` or `string basePath` string parameter. Replace `server.MapPath("~/...")` calls with `Path.Combine(env.WebRootPath, "...")` or `Path.Combine(env.ContentRootPath, "...")`.
2. Update all callers of PdfGenerator.RenderCompletePDF to pass the new parameter.
3. TopScorer.asmx / .asmx.cs and TopScorers.ashx / .ashx.cs: These ASMX/ASHX files are not supported in ASP.NET Core. Remove the code-behind files and exclude the asmx/ashx files from the project (they can remain on disk as documentation but should not be compiled).
4. Clean up Web.config — keep it as a reference but remove the compilation target reference. EF6 on .NET 8 needs the connection string accessible; if ConfigurationManager is not available, register the connection string in appsettings.json and configure EF6 to use it.
5. EF6 initialization: remove `Database.SetInitializer` if present (or set to null). Register the DbContext with the connection string from IConfiguration if used.

**Done when**: No HttpServerUtilityBase references remain; ASMX/ASHX code-behind files removed or excluded; solution builds with 0 errors and 0 warnings; EF6 connects correctly.
