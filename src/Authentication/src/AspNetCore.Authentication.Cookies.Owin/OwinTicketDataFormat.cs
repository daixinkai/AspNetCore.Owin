using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Authentication
{
    public class OwinTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly IDataSerializer<AuthenticationTicket> _serializer;
        private readonly IDataProtector _protector;

        public OwinTicketDataFormat(IDataSerializer<AuthenticationTicket> serializer, IDataProtector protector)
        {
            _serializer = serializer;
            _protector = protector;
        }


        public string Protect(AuthenticationTicket data)
        {
            return Protect(data, null);
        }


        public string Protect(AuthenticationTicket data, string purpose)
        {
            byte[] plaintext = _serializer.Serialize(data);
            IDataProtector dataProtector = _protector;
            if (!string.IsNullOrEmpty(purpose))
            {
                dataProtector = dataProtector.CreateProtector(purpose);
            }
            return Base64UrlTextEncoder.Encode(dataProtector.Protect(plaintext));
        }


        public AuthenticationTicket Unprotect(string protectedText)
        {
            return Unprotect(protectedText, null);
        }


        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            AuthenticationTicket tdata;
            try
            {
                if (protectedText == null)
                {
                    tdata = default(AuthenticationTicket);
                }
                else
                {
                    byte[] array = Base64UrlTextEncoder.Decode(protectedText);
                    if (array == null)
                    {
                        tdata = default(AuthenticationTicket);
                    }
                    else
                    {
                        IDataProtector dataProtector = _protector;
                        if (!string.IsNullOrEmpty(purpose))
                        {
                            dataProtector = dataProtector.CreateProtector(purpose);
                        }
                        byte[] array2 = dataProtector.Unprotect(array);
                        if (array2 == null)
                        {
                            tdata = default(AuthenticationTicket);
                        }
                        else
                        {
                            tdata = _serializer.Deserialize(array2);
                        }
                    }
                }
            }
            catch
            {
                tdata = default(AuthenticationTicket);
            }
            return tdata;
        }



    }
}
