# Upgrade Options — MangesTips (Tips.sln)

Assessment: 1 project (Tips.csproj, net48, WAP); 324 issues (309 ASP.NET Framework/System.Web, 2 Legacy Config); EF6 6.5.1; 20 affected files; 5 NuGet packages (0 incompatible, 1 upgrade recommended)

## Strategy

### Upgrade Strategy
Single project solution — no dependency graph to manage; All-at-Once is the fixed strategy.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade the single project in one atomic pass. Fastest approach with no multi-targeting overhead. |

## Project Structure

### Project Approach
Tips.csproj is an ASP.NET Framework MVC web project (System.Web). Multi-targeting is not viable; choice is between side-by-side and in-place rewrite.

| Value | Description |
|-------|-------------|
| **In-place rewrite** (selected) | Replace the Framework web project entirely in one pass. Appropriate for a single-project solution where the team controls the migration window. |
| Side-by-side | Create a new ASP.NET Core project alongside the old one; migrate assets incrementally while old project stays live. |

## Compatibility

### System.Web Adapters
ASP.NET Framework MVC (System.Web) detected with 309 issues. In-place rewrite selected, so compatibility shims are not needed.

| Value | Description |
|-------|-------------|
| **Direct Migration to ASP.NET Core APIs** (selected) | Replace all System.Web usage directly with native ASP.NET Core equivalents. Cleaner result with no compatibility layer to remove later. |
| Use System.Web Adapters | Add Microsoft.AspNetCore.SystemWebAdapters for HttpContext.Current shims. Enables incremental migration; requires cleanup pass afterward. |

### Unsupported API Handling
Binary (Api.0001) and source (Api.0002) incompatible APIs detected. Inline fixing is the default for single-project upgrades.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve every API change in the same task, including complex ones. No deferred stubs to clean up later. |
| Defer Complex Changes | Apply simple replacements inline; stub complex changes and create resolution subtasks to tackle them separately. |

## Modernization

### Entity Framework
EntityFramework 6.5.1 detected. Upgrading to EF Core simultaneously with the .NET upgrade introduces two sources of breaking changes; sequencing them separately is safer.

| Value | Description |
|-------|-------------|
| **Keep EF6** (selected) | EF6 6.3+ is compatible with .NET Core. Complete the .NET upgrade first, then migrate to EF Core as a separate follow-on effort. |
| Migrate to EF Core | Migrate EF6 to EF Core simultaneously with the .NET upgrade. Higher risk; only appropriate for small, simple data layers. |

### Nullable Reference Types
Single large project with 324 issues — migration is already demanding. Enable nullable separately after the upgrade settles.

| Value | Description |
|-------|-------------|
| **Leave Disabled** (selected) | Does not enable nullable reference types. Maintain existing null handling; enable as a separate effort after migration. |
| Enable Nullable Reference Types | Add `<Nullable>enable</Nullable>` to the project file. Enables compile-time null safety; may require code updates. |
