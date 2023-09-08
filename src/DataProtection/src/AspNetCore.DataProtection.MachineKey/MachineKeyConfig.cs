using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    public class MachineKeyConfig
    {
        private const string OBSOLETE_CRYPTO_API_MESSAGE = "This API exists only for backward compatibility; new framework features that require cryptographic services MUST NOT call it. New features should use the AspNetCryptoServiceProvider class instead.";

        // If the default validation algorithm changes, be sure to update the _HashSize and _AutoGenValidationKeySize fields also.
        internal const string DefaultValidationAlgorithm = "HMACSHA256";
        internal const MachineKeyValidation DefaultValidation = MachineKeyValidation.SHA1;
        internal const string DefaultDataProtectorType = "";
        internal const string DefaultApplicationName = "";


        #region property

        private string _decryption;

        public string Decryption
        {
            get
            {
                string s = GetDecryptionAttributeSkipValidation();
                //if (s != "Auto" && s != "AES" && s != "3DES" && s != "DES" && !s.StartsWith(ALGO_PREFIX, StringComparison.Ordinal))
                //    throw new ConfigurationErrorsException(SR.GetString(SR.Wrong_decryption_enum), ElementInformation.Properties["decryption"].Source, ElementInformation.Properties["decryption"].LineNumber);
                return s;
            }
            set
            {
                _decryption = value;
            }
        }

        public string DecryptionKey { get; set; }

        private bool _validationIsCached;
        private string _validation;

        private MachineKeyValidation _cachedValidationEnum;

        public MachineKeyValidation Validation
        {
            get
            {
                if (_validationIsCached == false)
                    CacheValidation();
                return _cachedValidationEnum;
            }
            set
            {
                if (_validationIsCached && value == _cachedValidationEnum)
                    return;
                _validation = MachineKeyValidationConverter.ConvertFromEnum(value);
                _cachedValidationEnum = value;
                _validationIsCached = true;
            }
        }

        public string ValidationAlgorithm
        {
            get
            {
                return Validation.ToString();
            }
        }

        private void CacheValidation()
        {
            _validation = GetValidationAttributeSkipValidation();
            _cachedValidationEnum = MachineKeyValidationConverter.ConvertToEnum(_validation);
            _validationIsCached = true;
        }

        public string ValidationKey { get; set; }

        public string DataProtectorType { get; set; }

        public string ApplicationName { get; set; }
        #endregion

        #region method
        internal string GetDecryptionAttributeSkipValidation()
        {
            return _decryption ?? "Auto";
        }
        internal string GetValidationAttributeSkipValidation()
        {
            return _validation ?? DefaultValidationAlgorithm;
        }
        #endregion

    }
}
