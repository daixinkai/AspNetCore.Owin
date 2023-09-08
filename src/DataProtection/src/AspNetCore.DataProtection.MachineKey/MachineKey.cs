using Microsoft.AspNetCore.DataProtection.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    public class MachineKey
    {
        public MachineKey()
        {
        }
        public MachineKey(MachineKeyConfig machineKeyConfig)
        {
            _machineKeyConfig = machineKeyConfig;
        }

        private MachineKeyConfig _machineKeyConfig;

        public MachineKeyConfig MachineKeyConfig
        {
            get
            {
                if (_machineKeyConfig == null)
                {
                    _machineKeyConfig = new MachineKeyConfig();
                }
                return _machineKeyConfig;
            }
            set
            {
                _machineKeyConfig = value;
                _cryptoServiceProvider = null;
            }
        }


        private ICryptoServiceProvider _cryptoServiceProvider;

        private ICryptoServiceProvider CryptoServiceProvider
        {
            get
            {
                if (_cryptoServiceProvider == null)
                {
                    _cryptoServiceProvider = AspNetCoreCryptoServiceProvider.GetCryptoServiceProvider(MachineKeyConfig);
                }
                return _cryptoServiceProvider;
            }
        }

        public string PrimaryPurpose
        {
            get
            {
                return Purpose.PrimaryPurpose;
            }
            set
            {
                _primaryPurpose = value;
                _purpose = null;
            }
        }


        private string _primaryPurpose;

        private Purpose _purpose;
        private Purpose Purpose
        {
            get
            {
                if (_purpose == null)
                {
                    if (string.IsNullOrWhiteSpace(_primaryPurpose))
                    {
                        _purpose = Microsoft.AspNetCore.DataProtection.Cryptography.Purpose.User_MachineKey_Protect;
                    }
                    else
                    {
                        _purpose = new Microsoft.AspNetCore.DataProtection.Cryptography.Purpose(_primaryPurpose);
                    }
                }
                return _purpose;
            }
        }


        /// <summary>
        /// Cryptographically protects and tamper-proofs the specified data.
        /// </summary>
        /// <param name="userData">The plaintext data that needs to be protected.</param>
        /// <param name="purposes">(optional) A list of purposes that describe what the data is meant for.
        /// If this value is specified, the same list must be passed to the Unprotect method in order
        /// to decipher the returned ciphertext.</param>
        /// <returns>The ciphertext data. To decipher the data, call the Unprotect method, passing this
        /// value as the 'protectedData' parameter.</returns>
        /// <remarks>
        /// This method supercedes the Encode method, which required the caller to know whether he wanted
        /// the plaintext data to be encrypted, signed, or both. In contrast, the Protect method just
        /// does the right thing and securely protects the data. Ciphertext data produced by this method
        /// can only be deciphered by the Unprotect method.
        /// 
        /// The 'purposes' parameter is an optional list of reason strings that can lock the ciphertext
        /// to a specific purpose. The intent of this parameter is that different subsystems within
        /// an application may depend on cryptographic operations, and a malicious client should not be
        /// able to get the result of one subsystem's Protect method and feed it as input to another
        /// subsystem's Unprotect method, which could have undesirable or insecure behavior. In essence,
        /// the 'purposes' parameter helps ensure that some protected data can be consumed only by the
        /// component that originally generated it. Applications should take care to ensure that each
        /// subsystem uses a unique 'purposes' list.
        ///
        /// For example, to protect or unprotect an authentication token, the application could call:
        /// MachineKey.Protect(..., "Authentication token");
        /// MachineKey.Unprotect(..., "Authentication token");
        /// 
        /// Applications may dynamically generate the 'purposes' parameter if desired. If an application
        /// does this, user-supplied values like usernames should never directly be passed for the 'purposes'
        /// parameter. They should instead be prefixed with something (like "Username: " + username) to
        /// minimize the risk of a malicious client crafting input that collides with a token in use by some
        /// other part of the system. Any dynamically-generated tokens should come after non-dynamically
        /// generated tokens.
        /// 
        /// For example, to protect or unprotect a private message that is tied to a specific user, the
        /// application could call:
        /// MachineKey.Protect(..., "Private message", "Recipient: " + username);
        /// MachineKey.Unprotect(..., "Private message", "Recipient: " + username);
        /// 
        /// In both of the above examples, is it important that the caller of the Unprotect method be able to
        /// resurrect the original 'purposes' list. Otherwise the operation will fail with a CryptographicException.
        /// </remarks>
        public virtual byte[] Protect(byte[] userData, params string[] purposes)
        {
            if (userData == null)
            {
                throw new ArgumentNullException("userData");
            }

            // Technically we don't care if the purposes array contains whitespace-only entries,
            // but the DataProtector class does, so we'll just block them right here.
            if (purposes != null && purposes.Any(String.IsNullOrWhiteSpace))
            {
                throw new ArgumentException(SR.GetString(SR.MachineKey_InvalidPurpose), "purposes");
            }

            return Protect(CryptoServiceProvider, userData, purposes);
        }

        // Internal method for unit testing.
        internal byte[] Protect(ICryptoServiceProvider cryptoServiceProvider, byte[] userData, string[] purposes)
        {
            // If the user is calling this method, we want to use the ICryptoServiceProvider
            // regardless of whether or not it's the default provider.

            Purpose derivedPurpose = Purpose.AppendSpecificPurposes(purposes);
            ICryptoService cryptoService = cryptoServiceProvider.GetCryptoService(derivedPurpose);
            return cryptoService.Protect(userData);
        }

        /// <summary>
        /// Verifies the integrity of and deciphers the given ciphertext.
        /// </summary>
        /// <param name="protectedData">Ciphertext data that was produced by the Protect method.</param>
        /// <param name="purposes">(optional) A list of purposes that describe what the data is meant for.</param>
        /// <returns>The plaintext data.</returns>
        /// <exception>Throws a CryptographicException if decryption fails. This can occur if the 'protectedData' has
        /// been tampered with, if an incorrect 'purposes' parameter is specified, or if an application is deployed
        /// to more than one server (as in a farm scenario) but is using auto-generated encryption keys.</exception>
        /// <remarks>See documentation on the Protect method for more information.</remarks>
        public virtual byte[] Unprotect(byte[] protectedData, params string[] purposes)
        {
            if (protectedData == null)
            {
                throw new ArgumentNullException("protectedData");
            }

            // Technically we don't care if the purposes array contains whitespace-only entries,
            // but the DataProtector class does, so we'll just block them right here.
            if (purposes != null && purposes.Any(String.IsNullOrWhiteSpace))
            {
                throw new ArgumentException(SR.GetString(SR.MachineKey_InvalidPurpose), "purposes");
            }

            return Unprotect(CryptoServiceProvider, protectedData, purposes);
        }

        // Internal method for unit testing.
        internal byte[] Unprotect(ICryptoServiceProvider cryptoServiceProvider, byte[] protectedData, string[] purposes)
        {
            // If the user is calling this method, we want to use the ICryptoServiceProvider
            // regardless of whether or not it's the default provider.

            Purpose derivedPurpose = Purpose.AppendSpecificPurposes(purposes);
            ICryptoService cryptoService = cryptoServiceProvider.GetCryptoService(derivedPurpose);
            return cryptoService.Unprotect(protectedData);
        }


#if NET461
        public static MachineKey GetWebConfigMachineKey()
        {
            return new WebConfigMachineKey();
        }
#endif


    }
}
