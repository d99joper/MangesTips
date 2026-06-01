# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade Tips.csproj from .NET Framework 4.8 to .NET 8.0 LTS  
**Scope**: Single ASP.NET MVC web project (~20 affected files, 324 issues)

### Selected Strategy
**All-At-Once** — Single project upgraded in one atomic pass.  
**Rationale**: 1 project, no dependency graph to manage. In-place rewrite approach chosen for the web project.

---

## Tasks

### 01-prerequisites: Verify toolchain and SDK readiness

Confirm the local environment has the .NET 8 SDK installed and that global.json (if present) is compatible with .NET 8. Verify that no pending tool or environment changes will interfere with the upgrade. This is a fast gate task — no code changes are expected.

**Done when**: `dotnet --list-sdks` shows a .NET 8 SDK installed; any global.json present in the repo root is compatible with .NET 8.0; build environment is confirmed ready.

---

### 02-sdk-conversion: Convert Tips.csproj to SDK-style format

Tips.csproj currently uses the legacy project file format (old-style csproj with `<Project ToolsVersion=...>` and `packages.config`). Convert it to SDK-style format while keeping the target framework at `net48`. This is a structural change only — no TFM changes or API fixes in this task.

The conversion includes migrating from `packages.config` to `PackageReference` references. The four ASP.NET packages that are now included in framework references (Microsoft.AspNet.Mvc 5.2.9, Microsoft.AspNet.Razor 3.2.9, Microsoft.AspNet.WebPages 3.2.9, Microsoft.Web.Infrastructure 2.0.0) should be removed as part of this step since they will be flagged as redundant. EntityFramework 6.5.1 should be retained as a `PackageReference`.

**Done when**: Tips.csproj is SDK-style (`<Project Sdk="...">`); PackageReference entries replace packages.config; project builds successfully on `net48` with no new errors introduced.

---

### 03-upgrade-tips: Upgrade Tips.csproj to .NET 8 and migrate to ASP.NET Core

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

---

### 04-validation: Final validation and post-upgrade documentation

Run the full solution build and any available tests to confirm the upgrade is complete and stable. Document any deferred recommendations (e.g., future EF Core migration, enabling nullable reference types) for the user to action separately.

**Done when**: Solution builds clean with 0 errors and 0 warnings on `net8.0`; all tests pass (or failures are pre-existing and documented); a brief post-upgrade note is written summarizing deferred items (EF Core migration, nullable reference types).
