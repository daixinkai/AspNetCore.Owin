using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    public sealed class MachineKeyDataProtectionOptions
    {
        public string PrimaryPurpose { get; set; }
        public MachineKey MachineKey { get; set; }
    }
}
