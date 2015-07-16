using QPP.Api;
using QPP.Context;
using QPP.Diagnostic;
using QPP.Dialog;
using QPP.Localization;
using QPP.Messaging;
using QPP.Navigation;
using QPP.Runtime;
using QPP.ServiceLocation;
using QPP.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP
{
    /// <summary>
    /// 運行時上下文
    /// </summary>
    public class RuntimeContext
    {
        internal static IServiceLocator Locator { get; set; }

        /// <summary>
        /// 運行時服務
        /// </summary>
        public static IRuntimeService Service { get; private set; }

        /// <summary>
        /// 應該上下文
        /// </summary>
        public static IAppContext AppContext { get; private set; }

        public static RuntimeServiceBuilder Builder()
        {
            return new RuntimeServiceBuilder();
        }

        public static void SetCurrentService(IRuntimeService service)
        {
            Service = service;
        }

        public static void SetCurrentAppContext(IAppContext context)
        {
            AppContext = context;
        }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="newProvider"></param>
        public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
        {
            Locator = newProvider.Invoke();
        }
    }
}
