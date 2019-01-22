using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    // Represents an object that can provide DataProtector instances

    internal interface IDataProtectorFactory
    {

        DataProtector GetDataProtector(Purpose purpose);

    }
}
