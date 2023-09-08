using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    internal static class SR
    {

        public const string Cannot_impersonate = "Cannot_impersonate";
        public const string Wrong_decryption_enum = "Wrong_decryption_enum";
        public const string Invalid_decryption_key = "Invalid_decryption_key";
        public const string Wrong_validation_enum_FX45 = "Wrong_validation_enum_FX45";
        public const string Invalid_validation_key = "Invalid_validation_key";
        public const string MachineKeyDataProtectorFactory_FactoryCreationFailed = "MachineKeyDataProtectorFactory_FactoryCreationFailed";
        public const string MachineKey_InvalidPurpose = "MachineKey_InvalidPurpose";
        public const string Wrong_validation_enum = "Wrong_validation_enum";
        public static string GetString(string key)
        {
            return key;
        }
    }
}