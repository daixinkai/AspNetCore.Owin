#if NET461
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    sealed class WebConfigMachineKey : MachineKey
    {

        public override byte[] Protect(byte[] userData, params string[] purposes)
        {
            return System.Web.Security.MachineKey.Protect(userData, purposes);
        }

        public override byte[] Unprotect(byte[] protectedData, params string[] purposes)
        {
            return System.Web.Security.MachineKey.Unprotect(protectedData, purposes);
        }

    }
}

#endif