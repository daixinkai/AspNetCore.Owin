using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.Cryptography
{
    // The central ASP.NET class for providing ICryptoService instances.
    // Get an instance of this class via the static Instance property.

    internal sealed class AspNetCoreCryptoServiceProvider : ICryptoServiceProvider
    {

        private readonly ICryptoAlgorithmFactory _cryptoAlgorithmFactory;
        private readonly IDataProtectorFactory _dataProtectorFactory;
        private readonly bool _isDataProtectorEnabled;
        private KeyDerivationFunction _keyDerivationFunction;
        private readonly MachineKeyConfig _machineKeyConfig;
        private readonly IMasterKeyProvider _masterKeyProvider;

        // This constructor is used only for testing purposes and by the singleton provider
        // and should not otherwise be called during ASP.NET request processing.
        internal AspNetCoreCryptoServiceProvider(MachineKeyConfig machineKeyConfig = null, ICryptoAlgorithmFactory cryptoAlgorithmFactory = null, IMasterKeyProvider masterKeyProvider = null, IDataProtectorFactory dataProtectorFactory = null, KeyDerivationFunction keyDerivationFunction = null)
        {
            _machineKeyConfig = machineKeyConfig;
            _cryptoAlgorithmFactory = cryptoAlgorithmFactory;
            _masterKeyProvider = masterKeyProvider;
            _dataProtectorFactory = dataProtectorFactory;
            _keyDerivationFunction = keyDerivationFunction;

            // The DataProtectorCryptoService is active if specified as such in config
            _isDataProtectorEnabled = (machineKeyConfig != null && !String.IsNullOrWhiteSpace(machineKeyConfig.DataProtectorType));
        }


        // Returns a value indicating whether this crypto service provider is the default
        // provider for the current application.
        internal bool IsDefaultProvider
        {
            get;
            private set;
        }

        public ICryptoService GetCryptoService(Purpose purpose, CryptoServiceOptions options = CryptoServiceOptions.None)
        {
            ICryptoService cryptoService;
            if (_isDataProtectorEnabled && options == CryptoServiceOptions.None)
            {
                // We can only use DataProtector if it's configured and the caller didn't ask for any special behavior like cacheability
                cryptoService = GetDataProtectorCryptoService(purpose);
            }
            else
            {
                // Otherwise we fall back to using the <machineKey> algorithms for cryptography
                cryptoService = GetNetFXCryptoService(purpose, options);
            }

            // always homogenize errors returned from the crypto service
            return new HomogenizingCryptoServiceWrapper(cryptoService);
        }

        private DataProtectorCryptoService GetDataProtectorCryptoService(Purpose purpose)
        {
            // just return the ICryptoService directly
            return new DataProtectorCryptoService(_dataProtectorFactory, purpose);
        }

        private NetFXCryptoService GetNetFXCryptoService(Purpose purpose, CryptoServiceOptions options)
        {
            // Extract the encryption and validation keys from the provided Purpose object
            CryptographicKey encryptionKey = purpose.GetDerivedEncryptionKey(_masterKeyProvider, _keyDerivationFunction);
            CryptographicKey validationKey = purpose.GetDerivedValidationKey(_masterKeyProvider, _keyDerivationFunction);

            // and return the ICryptoService
            // (predictable IV turned on if the caller requested cacheable output)
            return new NetFXCryptoService(_cryptoAlgorithmFactory, encryptionKey, validationKey, predictableIV: (options == CryptoServiceOptions.CacheableOutput));
        }


        internal static AspNetCoreCryptoServiceProvider GetCryptoServiceProvider(MachineKeyConfig machineKeyConfig)
        {
            // Provides all of the necessary dependencies for an application-level
            // AspNetCryptoServiceProvider.

            return new AspNetCoreCryptoServiceProvider(
                machineKeyConfig: machineKeyConfig,
                cryptoAlgorithmFactory: new MachineKeyCryptoAlgorithmFactory(machineKeyConfig),
                masterKeyProvider: new MachineKeyMasterKeyProvider(machineKeyConfig),
                dataProtectorFactory: new MachineKeyDataProtectorFactory(machineKeyConfig),
                keyDerivationFunction: SP800_108.DeriveKey);
        }


    }
}
