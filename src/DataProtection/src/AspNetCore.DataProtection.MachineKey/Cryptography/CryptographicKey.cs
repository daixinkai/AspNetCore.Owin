using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    internal sealed class CryptographicKey
    {

        private readonly byte[] _keyMaterial;

        public CryptographicKey(byte[] keyMaterial)
        {
            _keyMaterial = keyMaterial;
        }

        // Returns the length of the key (in bits).
        public int KeyLength
        {
            get
            {
                return checked(_keyMaterial.Length * 8);
            }
        }

        // Extracts the specified number of bits at the specified offset
        // and returns a new CryptographicKey. This is not appropriate
        // for subkey derivation, but it can be used if this cryptographic
        // key is actually two keys (like encryption + validation)
        // concatenated together. Inputs are specified as bit lengths.
        public CryptographicKey ExtractBits(int offset, int count)
        {
            int offsetBytes = offset / 8;
            int countBytes = count / 8;

            byte[] newKey = new byte[countBytes];
            Buffer.BlockCopy(_keyMaterial, offsetBytes, newKey, 0, countBytes);
            return new CryptographicKey(newKey);
        }

        // Returns the raw key material as a byte array.
        public byte[] GetKeyMaterial()
        {
            return _keyMaterial;
        }

    }
}
