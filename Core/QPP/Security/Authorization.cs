using QPP.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public class Authorization
    {
        /// <summary>
        /// 獲取權限
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool GetPermission(PresenterMetadata metadata, string command)
        {
#if !Admin
            return true;
#else
            var key = metadata.ModuleMetadata.Code.ToLower();
            if (RuntimeContext.AppContext.User.Permission.ContainsKey(key))
                return RuntimeContext.AppContext.User.Permission[key].Contains(command);
            return false;
#endif
        }
    }
}
