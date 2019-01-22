using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    internal static class UnsafeNativeMethods
    {
        static internal readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public const int TOKEN_ALL_ACCESS = 0x000f01ff;
        public const int TOKEN_EXECUTE = 0x00020000;
        public const int TOKEN_READ = 0x00020008;
        public const int TOKEN_IMPERSONATE = 0x00000004;

        public const int ERROR_NO_TOKEN = 1008;


        internal enum CallISAPIFunc : int
        {
            GetSiteServerComment = 1,
            RestrictIISFolders = 2,
            CreateTempDir = 3,
            GetAutogenKeys = 4,
            GenerateToken = 5
        };

        [DllImport(ModName.KERNEL32_FULL_NAME, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport(ModName.ADVAPI32_FULL_NAME)]
        internal static extern int RevertToSelf();

        [DllImport(ModName.ADVAPI32_FULL_NAME)]
        internal static extern int SetThreadToken(IntPtr threadref, IntPtr token);

        [DllImport(ModName.ADVAPI32_FULL_NAME, SetLastError = true)]
        internal static extern int OpenThreadToken(IntPtr thread, int access, bool openAsSelf, ref IntPtr hToken);

        [DllImport(ModName.KERNEL32_FULL_NAME)]
        internal static extern IntPtr GetCurrentThread();
        [DllImport(ModName.ENGINE_FULL_NAME)]
        internal static extern int EcbCallISAPI(IntPtr pECB, UnsafeNativeMethods.CallISAPIFunc iFunction, byte[] bufferIn, int sizeIn, byte[] bufferOut, int sizeOut);
    }
}