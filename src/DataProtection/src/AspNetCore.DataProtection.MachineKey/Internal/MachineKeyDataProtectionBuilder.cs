using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Internal
{
    internal class MachineKeyDataProtectionBuilder : IMachineKeyDataProtectionBuilder
    {
        public MachineKeyDataProtectionOptions Options { get; set; }
    }
}
