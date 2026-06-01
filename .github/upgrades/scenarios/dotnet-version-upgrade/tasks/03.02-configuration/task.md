# 03.02-configuration: Migrate Web.config appSettings and connectionStrings to appsettings.json

## Objective
Migrate configuration from Web.config to ASP.NET Core appsettings.json and wire IConfiguration in Program.cs.

## Scope
- Tips\Web.config — source for appSettings and connectionStrings
- Tips\secrets\appSettings.config.template — inspect for keys used by the app
- Tips\secrets\connectionStrings.config.template — inspect for connection string format
- Tips\appsettings.json — create
- Tips\appsettings.Development.json — create for dev overrides
- Tips\Program.cs — wire IConfiguration, add services
- Tips\Controllers\AuthController.cs — uses ConfigurationManager.AppSettings (1 usage)

## Steps
1. Create appsettings.json with the appSettings keys and connection strings from Web.config/templates
2. Create appsettings.Development.json for development-specific overrides
3. In Program.cs add `builder.Configuration` (automatic with WebApplication)
4. Wire IConfiguration injection where needed
5. In AuthController.cs replace `ConfigurationManager.AppSettings[username]` with `IConfiguration[username]` (inject IConfiguration via constructor)
6. Keep EF6 connection string accessible via ConfigurationManager OR add a compatibility shim — EF6 on .NET 8 reads from ConfigurationManager by default so may need a fallback

**Done when**: appsettings.json exists with all config keys; AuthController uses IConfiguration; no ConfigurationManager references remain in controllers.
