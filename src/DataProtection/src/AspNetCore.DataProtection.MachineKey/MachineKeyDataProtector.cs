using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    public class MachineKeyDataProtector : IDataProtector
    {
        public MachineKeyDataProtector(MachineKey machineKey)
        {
            _purposes = new string[0];
            MachineKey = machineKey;
        }

        public MachineKeyDataProtector(MachineKey machineKey, string[] purposes)
        {
            _purposes = purposes;
            MachineKey = machineKey;
        }


        private string[] _purposes;

        public MachineKey MachineKey { get; }


        public void AddPurpose(string purpose)
        {
            if (string.IsNullOrWhiteSpace(purpose))
            {
                return;
            }
            _purposes = ConcatPurposes(_purposes, purpose);
        }

        private static string[] ConcatPurposes(string[] originalPurposes, string newPurpose)
        {
            if (originalPurposes != null && originalPurposes.Length > 0)
            {
                var newPurposes = new string[originalPurposes.Length + 1];
                Array.Copy(originalPurposes, 0, newPurposes, 0, originalPurposes.Length);
                newPurposes[originalPurposes.Length] = newPurpose;
                return newPurposes;
            }
            else
            {
                return new string[] { newPurpose };
            }
        }


        public IDataProtector CreateProtector(string purpose)
        {
            MachineKeyDataProtector machineKeyDataProtector = new MachineKeyDataProtector(MachineKey, _purposes);
            machineKeyDataProtector.AddPurpose(purpose);
            return machineKeyDataProtector;
        }

        public byte[] Protect(byte[] plaintext)
        {
            return MachineKey.Protect(plaintext, _purposes);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return MachineKey.Unprotect(protectedData, _purposes);
        }
    }
}
