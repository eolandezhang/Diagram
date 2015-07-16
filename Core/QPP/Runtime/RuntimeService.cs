using QPP.Api;
using QPP.Diagnostic;
using QPP.Dialog;
using QPP.Localization;
using QPP.Logging;
using QPP.Messaging;
using QPP.Navigation;
using QPP.Security;
using QPP.ServiceLocation;
using QPP.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    public class RuntimeService : IRuntimeService
    {
        static Dictionary<string, object> ServicesContainer = new Dictionary<string, object>();

        public virtual IMessenger Messenger { get { return GetObject<IMessenger>(); } }

        public virtual ITrace Trace { get { return GetObject<ITrace>(); } }

        public virtual ILog Logger { get { return GetObject<ILog>(); } }

        public virtual IExceptionHandler ExceptionHandler { get { return GetObject<IExceptionHandler>(); } }

        public virtual ILocalization L10N { get { return GetObject<ILocalization>(); } }

        public virtual IAsyncWorker Worker { get { return GetObject<IAsyncWorker>(); } }

        public IDictionary RegisteredService
        {
            get { return ServicesContainer; }
        }

        public IRuntimeService Register<T>(T service, string key = null)
        {
            if (key.IsNullOrEmpty())
                key = typeof(T).FullName;
            if (ServicesContainer.ContainsKey(key))
                throw new InvalidOperationException("Service key:{0} is registed.".FormatArgs(key));
            ServicesContainer.Add(key, service);
            return this;
        }

        public void Unegister<T>(string key = null)
        {
            if (key.IsNullOrEmpty())
                key = typeof(T).FullName;
            ServicesContainer.Remove(key);
        }

        public T GetObject<T>(string key = null)
        {
            bool isKeyNull = key.IsNullOrEmpty();
            if (isKeyNull)
                key = typeof(T).FullName;
            if (ServicesContainer.ContainsKey(key))
                return (T)ServicesContainer[key];
            if (RuntimeContext.Locator != null)
            {
                if (isKeyNull && RuntimeContext.Locator.Contains<T>())
                    return RuntimeContext.Locator.GetObject<T>();
                if (!isKeyNull && RuntimeContext.Locator.Contains(key))
                    return RuntimeContext.Locator.GetObject<T>(key);
            }
            return default(T);
        }

        public bool Contains<T>(string key = null)
        {
            bool isKeyNull = key.IsNullOrEmpty();
            if (isKeyNull)
                key = typeof(T).FullName;
            if (ServicesContainer.ContainsKey(key)) return true;

            if (RuntimeContext.Locator != null)
            {
                if (isKeyNull && RuntimeContext.Locator.Contains<T>())
                    return true;
                if (!isKeyNull && RuntimeContext.Locator.Contains(key))
                    return true;
            }
            return false;
        }
    }
}
