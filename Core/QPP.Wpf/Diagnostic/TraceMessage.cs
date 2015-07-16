using QPP.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Diagnostic
{
    public class TraceMessage: GenericMessage<string>
    {
        public TraceMessage(string content)
            : base(content)
        {

        }
    }
}
