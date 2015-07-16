using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP.Security
{
    public class SecurityException : AppException
    {
        public SecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public SecurityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public SecurityException(string message)
            : base(message)
        {
        }
    }
}
