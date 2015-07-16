using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP.Data
{
    public class ObjectNotFoundException : AppException
    {
        public ObjectNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public ObjectNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ObjectNotFoundException(string message)
            : base(message)
        {
        }
    }
}
