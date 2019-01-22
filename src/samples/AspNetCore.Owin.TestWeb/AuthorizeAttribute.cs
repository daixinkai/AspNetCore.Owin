using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Owin.TestWeb
{
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!AuthorizeCore(context.HttpContext))
            {
                HandleUnauthorizedRequest(context);
            }
        }

        protected virtual bool AuthorizeCore(HttpContext httpContext)
        {
            return httpContext.User.Identity.IsAuthenticated;
        }

        protected virtual void HandleUnauthorizedRequest(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new ChallengeResult();
        }

    }
}
