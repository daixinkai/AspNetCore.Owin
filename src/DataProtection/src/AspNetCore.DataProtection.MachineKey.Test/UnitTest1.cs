using AspNetCore.DataProtection.MachineKey.Test;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace Microsoft.AspNetCore.DataProtection.MachineKey.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var services = new ServiceCollection();

            services.AddMachineKeyDataProtection(options =>
            {
                options.PrimaryPurpose = "TestPrimaryPurpose";
            }).WithXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<machineKey decryptionKey=""AA058CC55FAD77E074B093A59683D81143C6570CD3257536F35351549E29B578"" validation=""HMACSHA256"" validationKey=""E13D8EED8DD0AE3ACCB638D0785A4AEE711804E3BEEFD83A564A64AFA642709FBC086F86D312CD6029368F2CAC4BE83253A0EFE21F1DCB6424B4386CCC91E5E1"" />");


            var serviceProvider = services.BuildServiceProvider();

            IDataProtectionProvider dataProtectionProvider = serviceProvider.GetService<IDataProtectionProvider>();

            Assert.IsNotNull(dataProtectionProvider);

            Assert.IsInstanceOfType(dataProtectionProvider, typeof(MachineKeyDataProtectionProvider));

            IDataProtector dataProtector = dataProtectionProvider.CreateProtector("test123");

            Ticket ticket = new Ticket
            {
                Name = "Michael",
                UserId = 1001001
            };

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
#pragma warning disable SYSLIB0011 // 类型或成员已过时
            formatter.Serialize(stream, ticket);
#pragma warning restore SYSLIB0011 // 类型或成员已过时
            stream.Position = 0;

            var buffer = stream.ToArray();

            var eBuffer = dataProtector.Protect(buffer);

            var dBuffer = dataProtector.Unprotect(eBuffer);


            stream.Close();

            stream = new MemoryStream();

            stream.Write(dBuffer, 0, dBuffer.Length);

            stream.Position = 0;

#pragma warning disable SYSLIB0011 // 类型或成员已过时
            Ticket ticket2 = formatter.Deserialize(stream) as Ticket;
#pragma warning restore SYSLIB0011 // 类型或成员已过时


            Assert.AreNotEqual(ticket, ticket2);

            Assert.AreEqual(ticket.UserId, ticket2.UserId);
            Assert.AreEqual(ticket.Name, ticket2.Name);

        }

    }
}
