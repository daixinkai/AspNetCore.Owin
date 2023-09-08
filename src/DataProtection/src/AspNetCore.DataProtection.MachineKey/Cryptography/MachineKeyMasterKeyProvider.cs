using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    // Gets this application's master keys from the <machineKey> element,
    // optionally going against the auto-gen keys if AutoGenerate has been specified.

    internal sealed class MachineKeyMasterKeyProvider : IMasterKeyProvider
    {

        private const int AUTOGEN_ENCRYPTION_OFFSET = 0;
        private const int AUTOGEN_ENCRYPTION_KEYLENGTH = 256; // AES-256
        private const int AUTOGEN_VALIDATION_OFFSET = AUTOGEN_ENCRYPTION_KEYLENGTH;
        private const int AUTOGEN_VALIDATION_KEYLENGTH = 256; // HMACSHA256

        private const string AUTOGEN_KEYDERIVATION_PRIMARYPURPOSE = "MachineKeyDerivation";
        private const string AUTOGEN_KEYDERIVATION_ISOLATEAPPS_SPECIFICPURPOSE = "IsolateApps";
        private const string AUTOGEN_KEYDERIVATION_ISOLATEBYAPPID_SPECIFICPURPOSE = "IsolateByAppId";

        private string _applicationId;
        private string _applicationName;
        private CryptographicKey _autogenKeys;
        private CryptographicKey _encryptionKey;
        private KeyDerivationFunction _keyDerivationFunction;
        private readonly MachineKeyConfig _machineKeyConfig;
        private CryptographicKey _validationKey;

        // the only required parameter is 'machineKeySection'; other parameters are just used for unit testing
        internal MachineKeyMasterKeyProvider(MachineKeyConfig machineKeySection, string applicationId = null, string applicationName = null, CryptographicKey autogenKeys = null, KeyDerivationFunction keyDerivationFunction = null)
        {
            _machineKeyConfig = machineKeySection;
            _applicationId = applicationId;
            _applicationName = applicationName;
            _autogenKeys = autogenKeys;
            _keyDerivationFunction = keyDerivationFunction;
        }

        private static void AddSpecificPurposeString(IList<string> specificPurposes, string key, string value)
        {
            specificPurposes.Add(key + ": " + value);
        }

        // Generates 'cryptographicKey' from either the raw key material specified in config
        // or from the auto-generated key found in the system registry, optionally performing
        // subkey derivation.
        private CryptographicKey GenerateCryptographicKey(string configAttributeName, string configAttributeValue, int autogenKeyOffset, int autogenKeyCount, string errorResourceString)
        {
            byte[] keyMaterial = CryptoUtil.HexToBinary(configAttributeValue);

            // If <machineKey> contained a valid key, just use it verbatim.
            if (keyMaterial != null && keyMaterial.Length > 0)
            {
                return new CryptographicKey(keyMaterial);
            }

            // Otherwise, we need to generate it.
            bool autoGenerate = false;
            bool isolateApps = false;
            bool isolateByAppId = false;

            if (configAttributeValue != null)
            {
                foreach (string flag in configAttributeValue.Split(','))
                {
                    switch (flag)
                    {
                        case "AutoGenerate":
                            autoGenerate = true;
                            break;

                        case "IsolateApps":
                            isolateApps = true;
                            break;

                        case "IsolateByAppId":
                            isolateByAppId = true;
                            break;

                        default:
                            throw new ConfigurationException(configAttributeName + " is null");
                    }
                }
            }

            if (!autoGenerate)
            {
                throw new ConfigurationException(configAttributeName + " is null");
            }

            // The key should be a subset of the auto-generated key (which is a concatenation of several keys)
            CryptographicKey keyDerivationKey = AutogenKeys.ExtractBits(autogenKeyOffset, autogenKeyCount);
            List<string> specificPurposes = new List<string>();

            if (isolateApps)
            {
                // Use the application name to derive a new cryptographic key
                AddSpecificPurposeString(specificPurposes, AUTOGEN_KEYDERIVATION_ISOLATEAPPS_SPECIFICPURPOSE, ApplicationName);
            }

            if (isolateByAppId)
            {
                // Use the application ID to derive a new cryptographic key
                AddSpecificPurposeString(specificPurposes, AUTOGEN_KEYDERIVATION_ISOLATEBYAPPID_SPECIFICPURPOSE, ApplicationId);
            }

            // Don't use the auto-gen key directly; derive a new one based on specified parameters.
            Purpose purpose = new Purpose(AUTOGEN_KEYDERIVATION_PRIMARYPURPOSE, specificPurposes.ToArray());
            return KeyDerivationFunction(keyDerivationKey, purpose);


        }


        internal string ApplicationName
        {
            get
            {
                if (_applicationName == null)
                {
                    _applicationName = /*HttpRuntime.AppDomainAppVirtualPath ?? */Process.GetCurrentProcess().MainModule.ModuleName;
                }
                return _applicationName;
            }
        }

        internal string ApplicationId
        {
            get
            {
                if (_applicationId == null)
                {
                    //_applicationId = HttpRuntime.AppDomainAppId;
                    _applicationId = AppDomain.CurrentDomain.Id.ToString();
                }
                return _applicationId;
            }
        }

        internal KeyDerivationFunction KeyDerivationFunction
        {
            get
            {
                if (_keyDerivationFunction == null)
                {
                    _keyDerivationFunction = SP800_108.DeriveKey;
                }
                return _keyDerivationFunction;
            }
        }

        private static byte[] GetAutogenKeys()
        {

            byte[] autogenKeys = new byte[1024];

#if !FEATURE_PAL // FEATURE_PAL does not enable cryptography
            byte[] bKeysRandom = new byte[autogenKeys.Length];
            byte[] bKeysStored = new byte[autogenKeys.Length];
            bool fGetStoredKeys = false;
            using (var randgen = RandomNumberGenerator.Create())
            {
                // Gernerate random keys
                randgen.GetBytes(bKeysRandom);

                // If getting stored keys via WorkerRequest object failed, get it directly
                if (!fGetStoredKeys)
                    fGetStoredKeys = (UnsafeNativeMethods.EcbCallISAPI(IntPtr.Zero, UnsafeNativeMethods.CallISAPIFunc.GetAutogenKeys,
                                                                       bKeysRandom, bKeysRandom.Length, bKeysStored, bKeysStored.Length) == 1);
                // If we managed to get stored keys, copy them in; else use random keys
                if (fGetStoredKeys)
                    Buffer.BlockCopy(bKeysStored, 0, autogenKeys, 0, autogenKeys.Length);
                else
                    Buffer.BlockCopy(bKeysRandom, 0, autogenKeys, 0, autogenKeys.Length);
            }
#endif // !FEATURE_PAL
            return autogenKeys;
        }

        internal CryptographicKey AutogenKeys
        {
            get
            {
                if (_autogenKeys == null)
                {
                    _autogenKeys = new CryptographicKey(GetAutogenKeys());
                }
                return _autogenKeys;
            }
        }


        public CryptographicKey GetEncryptionKey()
        {
            if (_encryptionKey == null)
            {
                _encryptionKey = GenerateCryptographicKey(
                    configAttributeName: "decryptionKey",
                    configAttributeValue: _machineKeyConfig.DecryptionKey,
                    autogenKeyOffset: AUTOGEN_ENCRYPTION_OFFSET,
                    autogenKeyCount: AUTOGEN_ENCRYPTION_KEYLENGTH,
                    errorResourceString: SR.Invalid_decryption_key);
            }
            return _encryptionKey;
        }

        public CryptographicKey GetValidationKey()
        {
            if (_validationKey == null)
            {
                _validationKey = GenerateCryptographicKey(
                    configAttributeName: "validationKey",
                    configAttributeValue: _machineKeyConfig.ValidationKey,
                    autogenKeyOffset: AUTOGEN_VALIDATION_OFFSET,
                    autogenKeyCount: AUTOGEN_VALIDATION_KEYLENGTH,
                    errorResourceString: SR.Invalid_validation_key);
            }
            return _validationKey;
        }

    }
}
