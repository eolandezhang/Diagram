using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Context;
using Spring.Context.Support;
using System.Collections;
using System.Reflection;
using QPP.Caching;

namespace QPP.ServiceLocation
{
    public class SpringIoc : IServiceLocator
    {
        IApplicationContext m_ctx;

        LocatorStrategyFactory m_strategy;

        public LocatorStrategyFactory StrategyFactory
        {
            get
            {
                if (m_strategy == null)
                {
                    if (m_ctx.ContainsObject(typeof(LocatorStrategyFactory).FullName))
                        m_strategy = m_ctx.GetObject<LocatorStrategyFactory>();
                    else
                        m_strategy = new LocatorStrategyFactory();
                }
                return m_strategy;
            }
        }

        public SpringIoc SetCache(ICache cache)
        {
            foreach (var strategy in StrategyFactory.GetStrategies())
                strategy.Cache = cache;
            return this;
        }

        public static SpringIoc Default { get; set; }

        static SpringIoc()
        {
            Default = new SpringIoc();
        }

        public SpringIoc()
        {
            m_ctx = ContextRegistry.GetContext();            
        }

        public bool Contains(string name)
        {
            if(m_ctx.ContainsObject(name))return true;

            //foreach (var strategy in StrategyFactory.GetStrategies())
            //    if (strategy.Contains(type))
            //        return true;
            return false;
        }

        public bool Contains<T>()
        {
            if (m_ctx.ContainsObject(typeof(T).FullName)) return true;

            foreach (var strategy in StrategyFactory.GetStrategies())
                if (strategy.Contains(typeof(T)))
                    return true;
            return false;
        }

        public T GetObject<T>(string name)
        {
            return m_ctx.GetObject<T>(name);
        }

        public T GetObject<T>()
        {
            var type = typeof(T);
            if (type.IsAbstract || type.IsInterface)
            {
                if (m_ctx.ContainsObject(type.FullName))
                    return GetObject<T>(type.FullName);

                foreach (var strategy in StrategyFactory.GetStrategies())
                    if (strategy.Contains(type))
                        return strategy.GetObject<T>();
            }
            return m_ctx.GetObject<T>();
        }

        public IDictionary<string, T> GetObjects<T>()
        {
            return m_ctx.GetObjects<T>();
        }
    }
}
