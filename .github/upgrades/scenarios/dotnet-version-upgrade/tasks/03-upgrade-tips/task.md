# 03-upgrade-tips: Upgrade Tips.csproj to .NET 8 and migrate to ASP.NET Core

This is the core upgrade task for the single project. It covers the full in-place migration of the ASP.NET Framework MVC application to ASP.NET Core on .NET 8.0.

**Scope and known issues from assessment:**
- Change `TargetFramework` from `net48` to `net8.0`
- 294 binary-incompatible API usages (Api.0001) — predominately `System.Web` and ASP.NET Framework APIs that have no direct equivalent and must be replaced with ASP.NET Core equivalents
- 19 source-incompatible API usages (Api.0002) — require code-level changes to compile
- Route registration via `RouteCollection` (2 occurrences, Feature.0002) — must be converted to `app.MapControllerRoute(...)` / attribute routing in ASP.NET Core
- Entity Framework initialization (Feature.0004) — classic `Database.SetInitializer` pattern must be updated for .NET Core; EF6 6.5.1 will be kept (not migrated to EF Core)
- Global.asax.cs application startup (Feature.1000) — startup logic must be moved to `Program.cs` and the new minimal hosting model; Global.asax and Global.asax.cs can then be removed
- All four ASP.NET Framework NuGet packages removed in task 02 are replaced by the ASP.NET Core framework reference

The executor should research the affected files, identify all `System.Web` usages, and work through them systematically. All API replacements are done inline (Fix Inline option confirmed). EF6 initialization pattern needs adjustment but EF6 itself is retained.

**Done when**: Tips.csproj targets `net8.0`; solution builds with 0 errors and 0 warnings; Global.asax.cs is cleaned up or removed; ASP.NET Core routing is in place; EF6 initializes correctly under the new hosting model; all 19 source-incompatible APIs are resolved.
