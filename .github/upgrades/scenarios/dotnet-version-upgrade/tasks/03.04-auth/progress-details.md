# 03.04-auth: Progress Details

## Already Done (in 03.01 and 03.02)

All auth migration work was completed as part of earlier subtasks:

### Program.cs (03.01)
- `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(...)` registered
- `app.UseAuthentication()` and `app.UseAuthorization()` added to pipeline
- Login path `/Auth/Login`, timeout 30 min, cookie name `appNameAuth` preserved from Web.config

### AuthController.cs (03.02 + 03.03)
- `FormsAuthentication.SetAuthCookie` → `HttpContext.SignInAsync` with `ClaimsPrincipal`
- `FormsAuthentication.SignOut` → `HttpContext.SignOutAsync`
- Login/Logout actions made async (`Task<IActionResult>`)
- All `System.Web.Security` usings removed
- `Microsoft.AspNetCore.Authentication`, `Microsoft.AspNetCore.Authentication.Cookies`, `System.Security.Claims` added

## Verification
- `Select-String` for `FormsAuthentication` and `System.Web.Security` in AuthController.cs → 0 matches
