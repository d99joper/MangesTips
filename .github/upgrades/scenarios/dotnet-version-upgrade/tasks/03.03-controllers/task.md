# 03.03-controllers: Migrate all MVC controllers from System.Web.Mvc to Microsoft.AspNetCore.Mvc

## Objective
Migrate all 12 MVC controllers from ASP.NET Framework (System.Web.Mvc) to ASP.NET Core (Microsoft.AspNetCore.Mvc).

## Controllers
- BaseController.cs — change `using System.Web.Mvc` to `using Microsoft.AspNetCore.Mvc`; OnActionExecuting signature change
- HomeController.cs, AnswersController.cs, DetailsController.cs, StatisticsController.cs, RulesController.cs, BlogController.cs, NyKupongController.cs, ConfirmController.cs, AwaitingConfirmationController.cs — change namespace import only (low complexity, minimal System.Web usage)
- AdminController.cs — has [Authorize] and 2 System.Web refs, inspect for additional concerns
- AuthController.cs — uses FormsAuthentication (handled in 03.04), change controller base import now

## Steps for each controller
1. Replace `using System.Web.Mvc` with `using Microsoft.AspNetCore.Mvc`
2. Replace `using System.Web` (and sub-namespaces) with equivalent ASP.NET Core using statements
3. Keep `Controller` base class (it exists in Microsoft.AspNetCore.Mvc)
4. Fix any changed method signatures (ActionExecutingContext namespace change, etc.)
5. RouteConfig.cs: delete the file — routing will be in Program.cs

## Known concerns
- [Authorize] attribute: same name in ASP.NET Core, just different namespace — update using
- UrlParameter.Optional: no equivalent in ASP.NET Core routing; the default route in Program.cs handles this via nullable {id?}
- ActionResult return types are compatible

**Done when**: All controllers compile with Microsoft.AspNetCore.Mvc; no System.Web.Mvc or System.Web.Http using statements remain in controllers; RouteConfig.cs deleted.
