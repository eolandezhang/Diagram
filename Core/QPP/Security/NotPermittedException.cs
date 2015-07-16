using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP.Security
{
    public class NotPermittedException : AppException
    {
        public NotPermittedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public NotPermittedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public NotPermittedException(string message)
            : base(message)
        {
        }
    }
}
