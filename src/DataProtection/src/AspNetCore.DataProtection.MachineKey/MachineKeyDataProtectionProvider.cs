using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    public class MachineKeyDataProtectionProvider : IDataProtectionProvider
    {
        public MachineKeyDataProtectionProvider(MachineKeyDataProtectionOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            Options = options;
        }
        public MachineKeyDataProtectionOptions Options { get; }

        public IDataProtector CreateProtector(string purpose)
        {
            MachineKeyDataProtector machineKeyDataProtector = new MachineKeyDataProtector(Options.MachineKey);
            machineKeyDataProtector.AddPurpose(purpose);
            return machineKeyDataProtector;
        }
    }
}
