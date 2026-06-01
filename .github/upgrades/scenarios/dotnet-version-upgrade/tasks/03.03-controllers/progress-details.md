# 03.03-controllers: Progress Details

## What Changed

### All Controllers — System.Web.Mvc → Microsoft.AspNetCore.Mvc
- BaseController.cs — replaced `using System.Web.Mvc`, `using Microsoft.AspNetCore.Mvc.Filters`; `OnActionExecuting` visibility changed from `protected override` to `public override` (ASP.NET Core requirement)
- HomeController, AnswersController, DetailsController, StatisticsController, RulesController, NyKupongController, ConfirmController, AwaitingConfirmationController — replaced `using System.Web.Mvc` with `using Microsoft.AspNetCore.Mvc`
- AdminController — replaced `using System.Web.Mvc` + `using System.Web.Script.Serialization`; replaced `JavaScriptSerializer` with `System.Text.Json.JsonSerializer`; added `using Microsoft.AspNetCore.Authorization` for `[Authorize]`
- AuthController — added `using Microsoft.Extensions.Configuration`, `using System.Threading.Tasks` (async signatures)
- BlogController — added `using Microsoft.AspNetCore.Authorization`; replaced deprecated `TimeZone.CurrentTimeZone` → `TimeZoneInfo.Local`; replaced `TimeZone.ToUniversalTime` → `TimeZoneInfo.ConvertTimeToUtc`; replaced `HttpUtility.HtmlEncode` → `WebUtility.HtmlEncode` (System.Net); removed `[ValidateInput(false)]` (not needed in ASP.NET Core)
- DetailsController — injected `IWebHostEnvironment` for upcoming PdfGenerator fix (03.05); updated `PdfGenerator.RenderCompletePDF` call to pass `_env` instead of `HttpContext.Server`

### Tips\ViewModels\AdminViewModels.cs
- Removed `[AllowHtml]` attribute (not needed in ASP.NET Core — HTML input is allowed by default)

### Tips\App_Start\RouteConfig.cs — deleted
- Routing is now in Program.cs (`MapControllerRoute`)

## Build Result

Only 3 files still failing: `TopScorer.asmx.cs`, `TopScorers.ashx.cs`, `SendEmail.cs` — all in Helpers, addressed in 03.05.
All controllers and ViewModels compile cleanly.
