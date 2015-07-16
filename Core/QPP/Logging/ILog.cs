using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Logging
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsTraceEnabled { get; }
        bool IsWarnEnabled { get; }

        void Debug(object message);
        void Error(object message);
        void Fatal(object message);
        void Info(object message);
        void Trace(object message);
        void Warn(object message);
    }
}
