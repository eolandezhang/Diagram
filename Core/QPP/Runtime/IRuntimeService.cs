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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    /// <summary>
    /// 運行時的服務，提供基礎服務對象和業務服務對象
    /// </summary>
    public interface IRuntimeService
    {
        /// <summary>
        /// 消息中介者
        /// </summary>
        IMessenger Messenger { get; }
        /// <summary>
        /// 輸出
        /// </summary>
        ITrace Trace { get; }
        /// <summary>
        /// 日誌
        /// </summary>
        ILog Logger { get; }
        /// <summary>
        /// 異常處理
        /// </summary>
        IExceptionHandler ExceptionHandler { get; }
        /// <summary>
        /// 本土化
        /// </summary>
        ILocalization L10N { get; }
        /// <summary>
        /// 異步工作者
        /// </summary>
        IAsyncWorker Worker { get; }
        /// <summary>
        /// 運行時註冊的服務，可以根據運行環境動態切換
        /// </summary>
        IDictionary RegisteredService { get; }
        /// <summary>
        /// 註冊服務
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="key"></param>
        IRuntimeService Register<T>(T service, string key = null);
        /// <summary>
        /// 註銷服務
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        void Unegister<T>(string key = null);
        /// <summary>
        /// 判斷是否已註冊服務
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains<T>(string key = null);
        /// <summary>
        /// 獲取服務對象，包括基礎服務對象和業務服務對象。
        /// 通常基礎服務對象通過註冊引入，可以根據運行環境進行切換，而業務服務對象由ICO注入。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        T GetObject<T>(string key = null);
    }
}
