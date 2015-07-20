using QPP.ComponentModel;
using QPP.Metadata;
using QPP.Modularity;
using QPP.Navigation;
using QPP.Runtime;
using QPP.Wpf.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf
{
    public static class RuntimeExpension
    {
        /// <summary>
        /// 打開model的明細
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="model"></param>
        public static void OpenDetails<T>(this IRuntimeContainer container, object model, string key = null) where T : IPresenter
        {
            if (model == null)
                container.OpenContent<T>(DataArgs.Create("action", "create"));
            else
                container.OpenContent<T>(DataArgs.Create("model", model));
        }

        /// <summary>
        /// 打開內容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T OpenContent<T>(this IRuntimeContainer container, DataArgs args = null, string key = null) where T : IPresenter
        {
            var descriptor = RuntimeContext.Service.GetObject<IMetadataDescriptor>();
            var metadata = descriptor.GetMetadata(typeof(T));
            return (T)container.Open(metadata.Uri, args, key);
        }

        /// <summary>
        /// 打開對話框
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="owner"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T OpenDialog<T>(this IRuntimeContainer container, object owner, DataArgs args = null) where T : IPresenter
        {
            var descriptor = RuntimeContext.Service.GetObject<IMetadataDescriptor>();
            var metadata = descriptor.GetMetadata(typeof(T));
            return (T)container.OpenDialog(owner, metadata.Uri, args);
        }
    }
}
