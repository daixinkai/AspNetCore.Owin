//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Microsoft.AspNetCore.DataProtection.Internal
//{
//    class MachineKeyDataProtectionOptionsSetup : IConfigureOptions<MachineKeyDataProtectionOptions>
//    {
//        private readonly IServiceProvider _services;

//        public MachineKeyDataProtectionOptionsSetup(IServiceProvider provider)
//        {
//            _services = provider;
//        }

//        public void Configure(MachineKeyDataProtectionOptions options)
//        {
//            options.ApplicationDiscriminator = _services.GetApplicationUniqueIdentifier();
//        }
//    }
//}
