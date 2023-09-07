using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OwinCookieServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddOwinCookie(this AuthenticationBuilder builder, int formartVersion)
        {
            return builder.AddOwinCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                formartVersion);
        }

        public static AuthenticationBuilder AddOwinCookie(this AuthenticationBuilder builder, string authenticationScheme, int formartVersion)
        {
            return builder.AddOwinCookie(
                authenticationScheme,
                formartVersion,
                configureOptions: null);
        }

        public static AuthenticationBuilder AddOwinCookie(this AuthenticationBuilder builder, int formartVersion, Action<CookieAuthenticationOptions> configureOptions)
        {
            return builder.AddOwinCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                formartVersion,
                configureOptions);
        }

        public static AuthenticationBuilder AddOwinCookie(this AuthenticationBuilder builder, string authenticationScheme, int formartVersion, Action<CookieAuthenticationOptions> configureOptions)
        {
            return builder.AddOwinCookie(
                authenticationScheme,
                displayName: null,
                formartVersion: formartVersion,
                configureOptions: configureOptions);
        }

        public static AuthenticationBuilder AddOwinCookie(this AuthenticationBuilder builder, string authenticationScheme, string displayName, int formartVersion, Action<CookieAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddSingleton<OwinCookieOptions>(new OwinCookieOptions() { FormatVersion = formartVersion });
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, OwinPostConfigureCookieAuthenticationOptions>());
            return builder.AddScheme<CookieAuthenticationOptions, CookieAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
