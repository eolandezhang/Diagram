using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Logging
{
    public class DebugLogger : ILog
    {
        public bool IsDebugEnabled { get { return true; } }

        public bool IsErrorEnabled { get { return true; } }

        public bool IsFatalEnabled { get { return true; } }

        public bool IsInfoEnabled { get { return true; } }

        public bool IsTraceEnabled { get { return true; } }

        public bool IsWarnEnabled { get { return true; } }

        public void Debug(object message)
        {
            Write(message);
        }

        public void Error(object message)
        {
            Write(message);
        }

        public void Fatal(object message)
        {
            Write(message);
        }

        public void Info(object message)
        {
            Write(message);
        }

        public void Trace(object message)
        {
            Write(message);
        }

        public void Warn(object message)
        {
            Write(message);
        }

        void Write(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
