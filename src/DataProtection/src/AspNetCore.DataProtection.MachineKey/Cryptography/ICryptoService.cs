using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    // Represents an object that can perform cryptographic operations.
    // Get an instance of this class via an ICryptoServiceProvider (like AspNetCryptoServiceProvider).

    internal interface ICryptoService
    {

        // Protects some data by applying appropriate cryptographic transformations to it.
        byte[] Protect(byte[] clearData);

        // Returns the unprotected form of some protected data by validating and undoing the cryptographic transformations that led to it.
        byte[] Unprotect(byte[] protectedData);

    }
}
