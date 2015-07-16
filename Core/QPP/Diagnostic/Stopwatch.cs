using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Diagnostic
{
    public class Stopwatch : DisposableBase, IStopwatch
    {
        System.Diagnostics.Stopwatch m_Stopwatch = new System.Diagnostics.Stopwatch();

        public Stopwatch()
        {
            Start();
        }

        public Stopwatch(string messageFormat)
        {
            MessageFormat = messageFormat;
            Start();
        }

        public long ElapsedMilliseconds
        {
            get { return m_Stopwatch.ElapsedMilliseconds; }
        }

        public string MessageFormat { get; set; }

        public void Stop()
        {
            m_Stopwatch.Stop();
            if (MessageFormat.IsNotEmpty())
            {
                var msg = MessageFormat.FormatArgs(ElapsedMilliseconds);
                RuntimeContext.Service.Trace.WriteLine(msg);
            }
        }

        public void Start()
        {
            m_Stopwatch.Start();
        }

        protected override void Cleanup()
        {
            Stop();
        }
    }
}
