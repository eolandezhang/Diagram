using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using QPP.Data;
using QPP.Collections;

namespace QPP
{
    [Serializable]
    [DataContract]
    public class NamedQuery
    {
        //public static NamedQuery Default { get; private set; }

        //static NamedQuery()
        //{
        //    Default = new NamedQuery("Criteria");
        //}

        public NamedQuery()
        {
            Parameters = new Dictionary<string, ParameterValue>();
        }

        public NamedQuery(string name)
            : this()
        {
            Name = name;
        }

        //public static NamedQuery Criteria(string criteria)
        //{
        //    return new NamedQuery("Criteria").AddParameter("Criteria", criteria);
        //}

        public NamedQuery AddParameter(string key, object value)
        {
            Parameters.Add(key, new ParameterValue(value));
            return this;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Dictionary<string, ParameterValue> Parameters { get; set; }

        public T GetParameter<T>(string key, T defaultValue = default(T))
        {
            if (Parameters.ContainsKey(key))
                return Parameters[key].Value.ConvertTo<T>(defaultValue);
            return defaultValue;
        }
    }
}
