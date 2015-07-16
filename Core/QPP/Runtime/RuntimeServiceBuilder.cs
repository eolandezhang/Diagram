using QPP.Api;
using QPP.Diagnostic;
using QPP.Dialog;
using QPP.Localization;
using QPP.Logging;
using QPP.Messaging;
using QPP.Navigation;
using QPP.Security;
using QPP.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    public class RuntimeServiceBuilder
    {
        RuntimeService context;

        public RuntimeServiceBuilder()
        {
            context = new RuntimeService();
        }

        public RuntimeService Build()
        {
            return context;
        }

        public RuntimeServiceBuilder RegisterMessenger(IMessenger messenger)
        {
            context.Register(messenger);
            return this;
        }

        public RuntimeServiceBuilder RegisterTrace(ITrace trace)
        {
            context.Register(trace);
            return this;
        }

        public RuntimeServiceBuilder RegisterLogger(ILog logger)
        {
            context.Register(logger);
            return this;
        }

        public RuntimeServiceBuilder RegisterExceptionHandler(IExceptionHandler handler)
        {
            context.Register(handler);
            return this;
        }

        public RuntimeServiceBuilder RegisterLocalization(ILocalization localization)
        {
            context.Register(localization);
            return this;
        }

        public RuntimeServiceBuilder RegisterAsyncWorker(IAsyncWorker worker)
        {
            context.Register(worker);
            return this;
        }
    }
}
