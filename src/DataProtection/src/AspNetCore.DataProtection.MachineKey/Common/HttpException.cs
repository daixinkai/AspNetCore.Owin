using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.DataProtection
{
    internal class HttpException : Exception
    {
        public HttpException(string message) : base(message) { }
    }
}
