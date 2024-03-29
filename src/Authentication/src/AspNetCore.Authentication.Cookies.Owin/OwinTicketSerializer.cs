﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Microsoft.AspNetCore.Authentication
{
    class OwinTicketSerializer : IDataSerializer<AuthenticationTicket>
    {

        public OwinTicketSerializer(int formatVersion)
        {
            _formatVersion = formatVersion;
        }

        private readonly int _formatVersion;



        public virtual byte[] Serialize(AuthenticationTicket model)
        {
            using (var memory = new MemoryStream())
            {
                using (var compression = new GZipStream(memory, CompressionLevel.Optimal))
                {
                    using (var writer = new BinaryWriter(compression))
                    {
                        Write(writer, model, _formatVersion);
                    }
                }
                return memory.ToArray();
            }
        }


        public virtual AuthenticationTicket Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var compression = new GZipStream(memory, CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReader(compression))
                    {
                        return Read(reader, _formatVersion);
                    }
                }
            }
        }

        public static void Write(BinaryWriter writer, AuthenticationTicket model, int formatVersion)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            writer.Write(formatVersion);
            ClaimsIdentity identity = model.Principal.Identity as ClaimsIdentity;
            writer.Write(identity.AuthenticationType);
            WriteWithDefault(writer, identity.NameClaimType, DefaultValues.NameClaimType);
            WriteWithDefault(writer, identity.RoleClaimType, DefaultValues.RoleClaimType);
            writer.Write(identity.Claims.Count());
            foreach (var claim in identity.Claims)
            {
                WriteWithDefault(writer, claim.Type, identity.NameClaimType);
                writer.Write(claim.Value);
                WriteWithDefault(writer, claim.ValueType, DefaultValues.StringValueType);
                WriteWithDefault(writer, claim.Issuer, DefaultValues.LocalAuthority);
                WriteWithDefault(writer, claim.OriginalIssuer, claim.Issuer);
            }

            //BootstrapContext bc = identity.BootstrapContext as BootstrapContext;
            //if (bc == null || string.IsNullOrWhiteSpace(bc.Token))
            //{
            //    writer.Write(0);
            //}
            //else
            //{
            //    writer.Write(bc.Token.Length);
            //    writer.Write(bc.Token);
            //}

            var bc = identity.BootstrapContext as string;
            if (bc == null || string.IsNullOrWhiteSpace(bc))
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(bc.Length);
                writer.Write(bc);
            }
            PropertiesSerializer.Default.Write(writer, model.Properties);
        }

        public static AuthenticationTicket Read(BinaryReader reader, int formatVersion)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (reader.ReadInt32() != formatVersion)
            {
                return null;
            }

            string authenticationType = reader.ReadString();
            string nameClaimType = ReadWithDefault(reader, DefaultValues.NameClaimType);
            string roleClaimType = ReadWithDefault(reader, DefaultValues.RoleClaimType);
            int count = reader.ReadInt32();
            var claims = new Claim[count];
            for (int index = 0; index != count; ++index)
            {
                string type = ReadWithDefault(reader, nameClaimType);
                string value = reader.ReadString();
                string valueType = ReadWithDefault(reader, DefaultValues.StringValueType);
                string issuer = ReadWithDefault(reader, DefaultValues.LocalAuthority);
                string originalIssuer = ReadWithDefault(reader, issuer);
                claims[index] = new Claim(type, value, valueType, issuer, originalIssuer);
            }
            var identity = new ClaimsIdentity(claims, authenticationType, nameClaimType, roleClaimType);
            int bootstrapContextSize = reader.ReadInt32();
            if (bootstrapContextSize > 0)
            {
                //identity.BootstrapContext = new BootstrapContext(reader.ReadString());
                identity.BootstrapContext = reader.ReadString();
            }

            AuthenticationProperties properties = PropertiesSerializer.Default.Read(reader);
            return new AuthenticationTicket(new ClaimsPrincipal(identity), properties, authenticationType);
        }

        private static void WriteWithDefault(BinaryWriter writer, string value, string defaultValue)
        {
            if (string.Equals(value, defaultValue, StringComparison.Ordinal))
            {
                writer.Write(DefaultValues.DefaultStringPlaceholder);
            }
            else
            {
                writer.Write(value);
            }
        }

        private static string ReadWithDefault(BinaryReader reader, string defaultValue)
        {
            string value = reader.ReadString();
            if (string.Equals(value, DefaultValues.DefaultStringPlaceholder, StringComparison.Ordinal))
            {
                return defaultValue;
            }
            return value;
        }

        private static class DefaultValues
        {
            public const string DefaultStringPlaceholder = "\0";
            public const string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
            public const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            public const string LocalAuthority = "LOCAL AUTHORITY";
            public const string StringValueType = "http://www.w3.org/2001/XMLSchema#string";
        }
    }
}
