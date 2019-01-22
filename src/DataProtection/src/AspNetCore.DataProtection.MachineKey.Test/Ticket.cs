using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection.MachineKey.Test
{
    [Serializable]
    class Ticket
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
