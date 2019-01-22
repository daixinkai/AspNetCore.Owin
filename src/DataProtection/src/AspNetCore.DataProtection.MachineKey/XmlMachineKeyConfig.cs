using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.AspNetCore.DataProtection
{
    class XmlMachineKeyConfig : MachineKeyConfig
    {

        public XmlMachineKeyConfig(FileInfo fileInfo)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileInfo.FullName);
            Init(xmlDocument);
        }

        public XmlMachineKeyConfig(string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            Init(xmlDocument);
        }

        public XmlMachineKeyConfig(Stream stream)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);
            Init(xmlDocument);
        }

        public XmlMachineKeyConfig(XmlDocument xmlDocument)
        {
            Init(xmlDocument);
        }

        void Init(XmlDocument xmlDocument)
        {
            Decryption = GetAttributeValue(xmlDocument.DocumentElement, "decryption");
            DecryptionKey = GetAttributeValue(xmlDocument.DocumentElement, "decryptionKey");
            string validation = GetAttributeValue(xmlDocument.DocumentElement, "validation");
            if (!string.IsNullOrWhiteSpace(validation))
            {
                Validation = (MachineKeyValidation)Enum.Parse(typeof(MachineKeyValidation), validation);
            }
            ValidationKey = GetAttributeValue(xmlDocument.DocumentElement, "validationKey");
            ApplicationName = GetAttributeValue(xmlDocument.DocumentElement, "applicationName");
            DataProtectorType = GetAttributeValue(xmlDocument.DocumentElement, "dataProtectorType");
        }

        string GetAttributeValue(XmlElement xmlElement, string key)
        {
            string value = xmlElement.GetAttribute(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return value;
        }

    }
}
