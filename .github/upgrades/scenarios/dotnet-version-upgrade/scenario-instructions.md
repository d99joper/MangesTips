# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net8.0 (.NET 8.0 LTS)

## Source Control
- **Source Branch**: main
- **Working Branch**: modernize-dotnet8
- **Commit Strategy**: After Each Task

## Strategy
**Selected**: All-at-Once
**Rationale**: Single project (Tips.csproj), no dependency graph. In-place rewrite of ASP.NET MVC to ASP.NET Core.

### Execution Constraints
- Single atomic upgrade — all changes applied in one pass across tasks 02-04
- Full solution build (0 errors, 0 warnings) must pass before each task is marked complete
- All API incompatibilities fixed inline — no deferred stubs

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Project Structure
- Project Approach: In-place rewrite

### Compatibility
- System.Web Adapters: Direct Migration to ASP.NET Core APIs
- Unsupported API Handling: Fix Inline

### Modernization
- Entity Framework: Keep EF6
- Nullable Reference Types: Leave Disabled
