DataProtection
===========================

## AspNetCore.DataProtection.MachineKey


```
 string machineKey=@"<?xml version=""1.0"" encoding=""utf-8"" ?>
            <machineKey decryptionKey=""AA058CC55FAD77E074B093A59683D81143C6570CD3257536F35351549E29B578"" validation=""HMACSHA256"" validationKey=""E13D8EED8DD0AE3ACCB638D0785A4AEE711804E3BEEFD83A564A64AFA642709FBC086F86D312CD6029368F2CAC4BE83253A0EFE21F1DCB6424B4386CCC91E5E1"" />";
 services.AddMachineKeyDataProtection().WithXml(machineKey);
``` 