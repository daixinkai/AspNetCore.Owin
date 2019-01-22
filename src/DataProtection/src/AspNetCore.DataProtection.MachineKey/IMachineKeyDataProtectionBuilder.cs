using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    public interface IMachineKeyDataProtectionBuilder
    {
        MachineKeyDataProtectionOptions Options { get; }
    }
}
