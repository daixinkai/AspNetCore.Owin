using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    public enum MachineKeyValidation
    {
        MD5,
        SHA1,
        TripleDES,
        AES,
        HMACSHA256,
        HMACSHA384,
        HMACSHA512,
        Custom
    }
}
