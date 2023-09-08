using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    internal class MachineKeyValidationConverter
    {
        internal static string ConvertFromEnum(MachineKeyValidation enumValue)
        {
            switch (enumValue)
            {
                case MachineKeyValidation.SHA1:
                    return "SHA1";
                case MachineKeyValidation.MD5:
                    return "MD5";
                case MachineKeyValidation.TripleDES:
                    return "3DES";
                case MachineKeyValidation.AES:
                    return "AES";
                case MachineKeyValidation.HMACSHA256:
                    return "HMACSHA256";
                case MachineKeyValidation.HMACSHA384:
                    return "HMACSHA384";
                case MachineKeyValidation.HMACSHA512:
                    return "HMACSHA512";
                default:
                    throw new ArgumentException(SR.GetString(SR.Wrong_validation_enum));
            }
        }

        internal static MachineKeyValidation ConvertToEnum(string strValue)
        {
            if (strValue == null)
                return MachineKeyConfig.DefaultValidation;

            switch (strValue)
            {
                case "SHA1":
                    return MachineKeyValidation.SHA1;
                case "MD5":
                    return MachineKeyValidation.MD5;
                case "3DES":
                    return MachineKeyValidation.TripleDES;
                case "AES":
                    return MachineKeyValidation.AES;
                case "HMACSHA256":
                    return MachineKeyValidation.HMACSHA256;
                case "HMACSHA384":
                    return MachineKeyValidation.HMACSHA384;
                case "HMACSHA512":
                    return MachineKeyValidation.HMACSHA512;
                default:
                    if (strValue.StartsWith("alg:", StringComparison.Ordinal))
                        return MachineKeyValidation.Custom;
                    throw new ArgumentException(SR.GetString(SR.Wrong_validation_enum));
            }
        }

    }
}
