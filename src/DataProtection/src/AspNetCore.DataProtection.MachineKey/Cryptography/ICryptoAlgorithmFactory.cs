using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    // Represents an object that can provide encryption + validation algorithm instances

    internal interface ICryptoAlgorithmFactory
    {

        // Gets a SymmetricAlgorithm instance that can be used for encryption / decryption
        SymmetricAlgorithm GetEncryptionAlgorithm();

        // Gets a KeyedHashAlgorithm instance that can be used for signing / validation
        KeyedHashAlgorithm GetValidationAlgorithm();

    }
}
