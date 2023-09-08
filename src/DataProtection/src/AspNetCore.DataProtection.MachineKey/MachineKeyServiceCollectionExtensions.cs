using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Extensions.DependencyInjection
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class MachineKeyServiceCollectionExtensions
    {
        public static IMachineKeyDataProtectionBuilder AddMachineKeyDataProtection(this IServiceCollection services)
        {
            return services.AddMachineKeyDataProtection(null);
        }
        public static IMachineKeyDataProtectionBuilder AddMachineKeyDataProtection(this IServiceCollection services, Action<MachineKeyDataProtectionOptions> setupAction)
        {
            MachineKeyDataProtectionOptions options = new MachineKeyDataProtectionOptions();
            if (setupAction != null)
            {
                setupAction.Invoke(options);
            }
            MachineKeyDataProtectionProvider machineKeyDataProtectionProvider = new MachineKeyDataProtectionProvider(options);
            services.TryAddSingleton<IDataProtectionProvider>(machineKeyDataProtectionProvider);
            return new MachineKeyDataProtectionBuilder
            {
                Options = options
            };
        }


#if NET461
        public static IMachineKeyDataProtectionBuilder WithWebConfig(this IMachineKeyDataProtectionBuilder builder)
        { 
            builder.Options.MachineKey = MachineKey.GetWebConfigMachineKey();
            return builder;
        }
#endif

        #region xml

        public static IMachineKeyDataProtectionBuilder WithXml(this IMachineKeyDataProtectionBuilder builder, XmlDocument xmlDocument)
        {
            return builder.WithMachineKeyConfig(new XmlMachineKeyConfig(xmlDocument));
        }


        public static IMachineKeyDataProtectionBuilder WithXml(this IMachineKeyDataProtectionBuilder builder, Stream xmlStream)
        {
            return builder.WithMachineKeyConfig(new XmlMachineKeyConfig(xmlStream));
        }

        public static IMachineKeyDataProtectionBuilder WithXmlFile(this IMachineKeyDataProtectionBuilder builder, string xmlPath)
        {
            return builder.WithMachineKeyConfig(new XmlMachineKeyConfig(new FileInfo(xmlPath)));
        }

        public static IMachineKeyDataProtectionBuilder WithXml(this IMachineKeyDataProtectionBuilder builder, string xml)
        {
            return builder.WithMachineKeyConfig(new XmlMachineKeyConfig(xml));
        }

        #endregion

        public static IMachineKeyDataProtectionBuilder WithMachineKey(this IMachineKeyDataProtectionBuilder builder, MachineKey machineKey)
        {
            builder.Options.MachineKey = machineKey;
            if (!string.IsNullOrWhiteSpace(builder.Options.PrimaryPurpose))
            {
                builder.Options.MachineKey.PrimaryPurpose = builder.Options.PrimaryPurpose;
            }
            return builder;
        }

        public static IMachineKeyDataProtectionBuilder WithMachineKeyConfig(this IMachineKeyDataProtectionBuilder builder, MachineKeyConfig machineKeyConfig)
        {
            return builder.WithMachineKey(new MachineKey(machineKeyConfig));
        }

    }
}
