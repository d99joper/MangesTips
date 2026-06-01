# 02-sdk-conversion: Convert Tips.csproj to SDK-style format

Tips.csproj currently uses the legacy project file format (old-style csproj with `<Project ToolsVersion=...>` and `packages.config`). Convert it to SDK-style format while keeping the target framework at `net48`. This is a structural change only — no TFM changes or API fixes in this task.

The conversion includes migrating from `packages.config` to `PackageReference` references. The four ASP.NET packages that are now included in framework references (Microsoft.AspNet.Mvc 5.2.9, Microsoft.AspNet.Razor 3.2.9, Microsoft.AspNet.WebPages 3.2.9, Microsoft.Web.Infrastructure 2.0.0) should be removed as part of this step since they will be flagged as redundant. EntityFramework 6.5.1 should be retained as a `PackageReference`.

**Done when**: Tips.csproj is SDK-style (`<Project Sdk="...">`); PackageReference entries replace packages.config; project builds successfully on `net48` with no new errors introduced.
