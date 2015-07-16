using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Diagnostic
{
    public interface IStopwatch : IDisposable
    {
        long ElapsedMilliseconds { get; }
        string MessageFormat { get; }
        void Stop();
        void Start();
    }
}
