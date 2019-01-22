using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    // Represents an object that can provide ICryptoService instances.
    // Get an instance of this type via the AspNetCryptoServiceProvider.Instance singleton property.

    internal interface ICryptoServiceProvider
    {

        ICryptoService GetCryptoService(Purpose purpose, CryptoServiceOptions options = CryptoServiceOptions.None);

    }
}
