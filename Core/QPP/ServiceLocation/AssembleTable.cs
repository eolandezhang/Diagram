using QPP.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QPP.ServiceLocation
{
    /// <summary>
    /// 根據程序集對應關係查找類型
    /// <para>
    /// 例如QPP.Master.IBLL對應QPP.Master.BLL,則定義在IBLL中的所有抽象類型都從BLL中去查找實現
    /// </para>
    /// </summary>
    public class AssembleTable : LocatorStrategy
    {
        object sync = new object();

        public Dictionary<string, string> Assembles { get; set; }

        protected Hashtable GetCache(Type serviceType)
        {
            var key = "AssembleTable_" + serviceType.Assembly.FullName;
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
            string assemble = type.Assembly.GetName().Name;
            if (Assembles.ContainsKey(assemble))
                return true;
            return false;
        }

        public override T GetObject<T>()
        {
            var type = typeof(T);
            var cache = GetCache(type);
            if (cache.ContainsKey(type.FullName))
                return (T)cache[type.FullName];

            string assemble = type.Assembly.GetName().Name;
            if (Assembles.ContainsKey(assemble))
                assemble = Assembles[assemble].ToSafeString();

            foreach (var t in Assembly.Load(assemble).GetTypes())
            {
                if (!t.IsAbstract && (type.IsAssignableFrom(t) || t.IsSubclassOf(type)))
                {
                    var obj = (T)Assembly.Load(assemble).CreateInstance(t.FullName);
                    try
                    {
                        cache.Add(type.FullName, obj);
                    }
                    catch { }
                    return obj;
                }
            }
            return default(T);
        }
    }
}
