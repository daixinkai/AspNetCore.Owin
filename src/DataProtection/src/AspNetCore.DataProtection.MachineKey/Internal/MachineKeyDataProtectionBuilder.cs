using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Internal
{
    class MachineKeyDataProtectionBuilder : IMachineKeyDataProtectionBuilder
    {
        public MachineKeyDataProtectionOptions Options { get; set; }
    }
}
