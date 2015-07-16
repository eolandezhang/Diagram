using QPP.Diagnostic;
using QPP.Wpf.Controls.Output;
using QPP.Wpf.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Diagnostic
{
    public class Trace : ITrace
    {
        public void Write(object message)
        {
            var msg = message.ToSafeString();
            FileTracer.Write(msg);
            Outputer.Default.Write(msg);
        }

        public void WriteLine(object message)
        {
            FileTracer.Write(message + Environment.NewLine);
            Outputer.Default.Write(message + Environment.NewLine);
        }
    }
}
