# 04-validation: Progress Details

## Build Validation
- **Final build**: ✅ 0 errors, 0 warnings on `net8.0`
- All EF6 artifacts removed and replaced with EF Core 8.x equivalents

## EF Core Migration (completed as part of this session)

### Packages
- Removed: `EntityFramework 6.5.2`, `System.Configuration.ConfigurationManager`
- Added: `Microsoft.EntityFrameworkCore.SqlServer 8.x`, `Microsoft.EntityFrameworkCore.Proxies 8.x`, `Microsoft.EntityFrameworkCore.Design 8.x`

### Files Changed
| File | Change |
|------|--------|
| `Tips.csproj` | Swapped packages |
| `Tips/Models/Tips.Context.cs` | Full rewrite: `DbContextOptions<Tips_Entities>` constructor, `ModelBuilder`, EF Core fluent API (`HasOne`, `IsRequired`, `OnDelete(DeleteBehavior.Restrict)`) |
| `Tips/Models/Tips.Context.Partial.cs` | Deleted (string constructor no longer valid) |
| `Tips/Models/Tips.Designer.cs` | Deleted (empty T4 stub) |
| `Tips/Models/Tips1.cs` | Deleted (empty T4 stub) |
| `Tips/Models/Tips.edmx` | Deleted (empty EDMX placeholder) |
| `Tips/Models/UserRepository.cs` | Removed `System.Data.Entity`, `ExecuteSqlCommand` → `ExecuteSqlRaw`, `Include(x.Select(...))` → `ThenInclude`, static methods → instance methods |
| `Tips/Models/MatchRepository.cs` | `System.Data.Entity` → `Microsoft.EntityFrameworkCore` |
| `Tips/Program.cs` | `AddScoped<Tips_Entities>` → `AddDbContext<Tips_Entities>(UseSqlServer + UseLazyLoadingProxies)` |
| `Tips/Controllers/StatisticsController.cs` | Injected `UserRepository`, replaced static `UserRepository.*` calls with instance calls |
| `Tips/Controllers/BaseController.cs` | `private _settingsRepo` → `protected` |
| `Tips/Controllers/NyKupongController.cs` | Fixed `new SettingsRepository()` → `_settingsRepo` |

## EF6 Remnants Check (all zero)
- `System.Data.Entity` usings: 0
- `HasRequired` / `HasOptional`: 0
- `WillCascadeOnDelete`: 0
- `DbModelBuilder`: 0
- `ExecuteSqlCommand`: 0
- `EntityFramework` package reference: removed

## Deferred Items
- **EF Core migrations baseline**: No migrations folder created. The existing database schema was built by EF6; you should create a baseline EF Core migration and mark it as applied (do not run it against the DB) once you verify the model matches the schema:
  ```powershell
  dotnet ef migrations add InitialCreate
  dotnet ef migrations script --idempotent --output baseline.sql
  ```
  Then manually insert only the `__EFMigrationsHistory` row into your database.
- **Nullable reference types**: Disabled (as per upgrade preferences). Can be enabled incrementally post-upgrade.
- **Lazy loading**: Enabled via `UseLazyLoadingProxies()`. All navigation properties are `virtual`. Consider moving to explicit `.Include()` chains for performance-sensitive queries in the future.
