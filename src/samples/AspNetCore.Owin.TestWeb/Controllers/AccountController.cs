using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Owin.TestWeb.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> Login(string returnUrl)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity"));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")));

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties { });
            return Redirect(returnUrl ?? "~/");
        }


    }
}