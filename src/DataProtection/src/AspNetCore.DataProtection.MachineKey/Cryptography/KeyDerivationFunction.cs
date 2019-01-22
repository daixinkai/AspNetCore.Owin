using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    // A delegate that represents a cryptographic key derivation function (KDF).
    // A KDF takes a master key (the key derivation key) and a purpose string,
    // producing a derived key in the process.
    internal delegate CryptographicKey KeyDerivationFunction(CryptographicKey keyDerivationKey, Purpose purpose);
}
