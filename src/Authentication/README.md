
Authentication
===========================

## AspNetCore.Authentication.Cookies.Owin


```
services.AddAuthentication("ApplicationCookie").AddOwinCookie("ApplicationCookie", 3, options =>
{
      options.Cookie.Name = OwinCookieAuthenticationDefaults.CookiePrefix + "ApplicationCookie";
});
``` 