using QPP.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ServiceLocation
{
    /// <summary>
    /// 類型查找策略，當不能從配置中準確定位類型時，需要根據策略去查找類型
    /// </summary>
    public abstract class LocatorStrategy
    {
        ICache m_Cache = null;

        public ICache Cache
        {
            get { return m_Cache ?? (m_Cache = new NonExpiringCache()); }
            set { m_Cache = value; }
        }

        public abstract bool Contains(Type type);

        public abstract T GetObject<T>();
    }
}
