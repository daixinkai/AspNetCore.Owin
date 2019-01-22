// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication.Cookies
{
    /// <summary>
    /// Used to setup defaults for all <see cref="CookieAuthenticationOptions"/>.
    /// </summary>
    public class OwinPostConfigureCookieAuthenticationOptions : IPostConfigureOptions<CookieAuthenticationOptions>
    {
        private readonly IDataProtectionProvider _dp;
        private readonly OwinCookieOptions _owinCookieOptions;
        public OwinPostConfigureCookieAuthenticationOptions(IDataProtectionProvider dataProtection, OwinCookieOptions owinCookieOptions)
        {
            _dp = dataProtection;
            _owinCookieOptions = owinCookieOptions;
        }

        /// <summary>
        /// Invoked to post configure a TOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, CookieAuthenticationOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            if (string.IsNullOrEmpty(options.Cookie.Name))
            {
                options.Cookie.Name = CookieAuthenticationDefaults.CookiePrefix + name;
            }
            if (options.TicketDataFormat == null)
            {
                var dataProtector = options.DataProtectionProvider.CreateProtector("Microsoft.Owin.Security.Cookies.CookieAuthenticationMiddleware", name, "v1");
                options.TicketDataFormat = new OwinTicketDataFormat(new OwinTicketSerializer(_owinCookieOptions.FormatVersion), dataProtector);
            }
            if (options.CookieManager == null)
            {
                options.CookieManager = new ChunkingCookieManager();
            }
            if (!options.LoginPath.HasValue)
            {
                options.LoginPath = CookieAuthenticationDefaults.LoginPath;
            }
            if (!options.LogoutPath.HasValue)
            {
                options.LogoutPath = CookieAuthenticationDefaults.LogoutPath;
            }
            if (!options.AccessDeniedPath.HasValue)
            {
                options.AccessDeniedPath = CookieAuthenticationDefaults.AccessDeniedPath;
            }
        }
    }
}
