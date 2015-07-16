using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QPP.ServiceLocation
{
    /// <summary>
    /// 遍歷所有程序集查找類型
    /// </summary>
    public class AssembleLocator : LocatorStrategy
    {
        object sync = new object();

        static List<Assembly> Assemblies = new List<Assembly>();

        static AssembleLocator()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (Attribute.IsDefined(assembly, typeof(AssemblyLocatorAttribute)))
                    Assemblies.Add(assembly);

            AppDomain.CurrentDomain.AssemblyLoad += (s, e) =>
            {
                if (Attribute.IsDefined(e.LoadedAssembly, typeof(AssemblyLocatorAttribute)))
                    Assemblies.Add(e.LoadedAssembly);
            };
        }

        protected Hashtable GetCache(Type serviceType)
        {
            var key = "AssembleLocator_" + serviceType.Assembly.FullName;
            var table = Cache.Get(key) as Hashtable;
            if (table == null)
            {
                lock (sync)
                {
                    table = Cache.Get(key) as Hashtable;
                    if (table == null)
                    {
                        table = new Hashtable();
                        Cache.Insert(key, table);
                    }
                }
            }
            return table;
        }

        public override bool Contains(Type type)
        {
            var cache = GetCache(type);
            if (cache.ContainsKey(type.FullName))
                return true;
            if (cache.ContainsKey(type.Assembly.FullName))
            {
                var assembly = (Assembly)cache[type.Assembly.FullName];
                if (Contains(assembly, type, cache))
                    return true;
            }
            foreach (var a in Assemblies)
                if (Contains(a, type, cache))
                    return true;
            return false;
        }

        /// <summary>
        /// 判斷是否包含類型的時候就創建好放在緩存中
        /// </summary>
        bool Contains(Assembly assembly, Type type, Hashtable cache)
        {
            foreach (var t in assembly.GetTypes())
            {
                if (!t.IsAbstract && (type.IsAssignableFrom(t) || t.IsSubclassOf(type)))
                {
                    var obj = Activator.CreateInstance(t);
                    try
                    {
                        //緩存類型
                        cache.Add(type.FullName, obj);
                        //緩存程序集
                        if (!cache.ContainsKey(assembly.FullName))
                            cache.Add(assembly.FullName, assembly);
                    }
                    catch { }
                    return true;
                }
            }
            return false;
        }

        public override T GetObject<T>()
        {
            var type = typeof(T);
            var cache = GetCache(type);
            if (cache.ContainsKey(type.FullName))
                return (T)cache[type.FullName];
            //如果緩存中沒有，則再找一次。
            foreach (var a in Assemblies)
            {
                foreach (var t in a.GetTypes())
                {
                    if (!t.IsAbstract && (type.IsAssignableFrom(t) || t.IsSubclassOf(type)))
                    {
                        var obj = (T)Activator.CreateInstance(t);
                        try
                        {
                            cache.Add(type.FullName, obj);
                        }
                        catch { }
                        return obj;
                    }
                }
            }
            return default(T);
        }
    }
}
