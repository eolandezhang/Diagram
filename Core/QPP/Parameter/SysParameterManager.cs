using QPP.ServiceLocation;
using QPP.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace QPP.Parameter
{
    public static class SysParameterManager
    {
        static readonly Dictionary<Type, ISysParameterKey> keys = new Dictionary<Type, ISysParameterKey>();

        public static IEnumerable<ISysParameterKey> Parameters
        {
            get { return keys.Values; }
        }

        //static SysParameterManager()
        //{
        //    var set = AppDomain.CurrentDomain.GetAssemblies();
        //    Dictionary<string, string> lst = new Dictionary<string, string>();
        //    foreach (Assembly assembly in set)
        //    {
        //        if (assembly.FullName.StartsWith("System")) continue;
        //        foreach (var t in assembly.GetTypes())
        //        {
        //            if (typeof(ISysParameterKey).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
        //            {
        //                var i = Activator.CreateInstance(t) as ISysParameterKey;
        //                if (lst.ContainsKey(i.Category + "$" + i.Name))
        //                    throw new ValidationException("Type {0} and Type {1} has the same category[{2}] and name[{3}]", lst[i.Category + "$" + i.Name], t.FullName, i.Category, i.Name);

        //                lst.Add(i.Category + "$" + i.Name, t.FullName);
        //                keys.Add(t, i);
        //            }
        //        }
        //    }
        //}

        public static void Register(ISysParameterKey key)
        {
            var t = key.GetType();
            var k = keys.Values.FirstOrDefault(p => (p.Category + "$" + p.Name).Equals(key.Category + "$" + key.Name));
            if (k != null)
                throw new ValidationException("Type {0} and Type {1} has the same category[{2}] and name[{3}]", k.GetType(), t.FullName, key.Category, key.Name);

            keys.Add(t, key);
        }

        public static T GetValue<T>(SysParameterKey<T> key) where T : ISysParameterValue
        {
            var values = RuntimeContext.Service.GetObject<ISysParameterService>().GetParameter(key.Id);
            if (values.Count == 0)
            {
                Initialize(new[] { key });
                return key.DefaultValue;
            }
            if (typeof(IList).IsAssignableFrom(typeof(T)))
            {
                var t = typeof(T).GetGenericArguments()[0];
                var lst = (IList)Activator.CreateInstance<T>();
                foreach (var m in values)
                {
                    var p = (SysParameterValue)Activator.CreateInstance(t);
                    p.Load(m);
                    lst.Add(p);
                }
                return (T)lst;
            }
            else
            {
                if (values.Count > 1)
                    throw new ValidationException("System parameter {0} found more than one settings", key.Name);
                var param = Activator.CreateInstance<T>();
                (param as SysParameterValue).Load(values.Single());
                return (T)param;
            }
        }

        public static T Keys<T>() where T : ISysParameterKey
        {
            var t = typeof(T);
            if (keys.ContainsKey(t))
                return (T)keys[t];
            var instanct = Activator.CreateInstance<T>();
            keys.Add(t, instanct);
            return instanct;
        }

        static void Initialize(IEnumerable<ISysParameterKey> keys)
        {
            var parameters = new List<SysParameterKeyModel>();
            foreach (var p in keys)
            {
                var parameter = new SysParameterKeyModel();
                parameter.Id = p.Id;
                parameter.Name = p.Name;
                parameter.Category = p.Category;
                parameter.Description = p.Description;
                parameter.Values = new List<SysParameterValueModel>();
                var defaultValue = p.GetType().GetProperty("DefaultValue");
                if (typeof(IList).IsAssignableFrom(defaultValue.PropertyType))
                {
                    var values = defaultValue.GetValue(p, null) as IList;
                    if (values != null)
                    {
                        foreach (SysParameterValue value in values)
                        {
                            var paramValue = value.Save();
                            paramValue.Key = parameter.Id;
                            parameter.Values.Add(paramValue);
                        }
                    }
                }
                else
                {
                    var value = defaultValue.GetValue(p, null) as SysParameterValue;
                    var paramValue = new SysParameterValueModel();
                    paramValue.Key = parameter.Id;
                    if (value != null)
                    {
                        var v = value.Save();
                        paramValue.Value1 = v.Value1;
                        paramValue.Value2 = v.Value2;
                        paramValue.Value3 = v.Value3;
                        paramValue.Value4 = v.Value4;
                        paramValue.Value5 = v.Value5;
                    }
                    parameter.Values.Add(paramValue);
                }
                parameters.Add(parameter);
            }
            RuntimeContext.Service.GetObject<ISysParameterService>().Initialize(parameters);
        }

        public static void Initialize()
        {
            Initialize(keys.Values);
        }
    }
}
