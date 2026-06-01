# 03.04-auth: Replace FormsAuthentication with ASP.NET Core cookie authentication

## Objective
Replace FormsAuthentication (System.Web.Security) with ASP.NET Core cookie authentication middleware.

## Scope
- Tips\Controllers\AuthController.cs — FormsAuthentication.SetAuthCookie, FormsAuthentication.SignOut (3 usages)
- Tips\Program.cs — add cookie auth services and middleware
- Web.config had: `<authentication mode="Forms"><forms loginUrl="~/Auth/Login" timeout="30" defaultUrl="~/Admin"/></authentication>`

## Steps
1. In Program.cs add: `builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => { options.LoginPath = "/Auth/Login"; options.ExpireTimeSpan = TimeSpan.FromMinutes(30); });`
2. Add `app.UseAuthentication()` and `app.UseAuthorization()` to the pipeline (before MapControllerRoute)
3. In AuthController Login POST: replace `FormsAuthentication.SetAuthCookie(username, false)` with `await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, CookieAuthenticationDefaults.AuthenticationScheme)))`
4. In AuthController Logout: replace `FormsAuthentication.SignOut()` with `await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)`
5. In AuthController Login GET: remove `FormsAuthentication.SignOut()` — not needed in Core (or replace with SignOutAsync)
6. Add `using Microsoft.AspNetCore.Authentication; using Microsoft.AspNetCore.Authentication.Cookies; using System.Security.Claims;` to AuthController
7. Make Login POST and Logout actions async (return Task<IActionResult>)
8. Add `Microsoft.AspNetCore.Authentication.Cookies` package if not already part of the SDK (it is included in ASP.NET Core SDK)

**Done when**: FormsAuthentication removed; cookie auth registered in Program.cs; AuthController uses SignInAsync/SignOutAsync; no System.Web.Security references remain.
