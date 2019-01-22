using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    internal enum CryptoServiceOptions
    {

        // [default] no special behavior needed
        None = 0,

        // the output of the Protect method will be cached, so the same plaintext should lead to the same ciphertext (no randomness)
        CacheableOutput,

    }
}
