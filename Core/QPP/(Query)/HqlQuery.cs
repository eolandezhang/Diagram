using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using QPP.Collections;
using QPP.Data;

namespace QPP
{
    [Serializable]
    [DataContract]
    public class HqlQuery
    {
        [DataMember]
        public string Hql { get; set; }

        [DataMember]
        public int SkipRows { get; set; }

        [DataMember]
        public int MaxRows { get; set; }

        [DataMember]
        public TupleList<string, ParameterValue> Parameters { get; set; }

        public HqlQuery()
        {
            MaxRows = -1;
            Parameters = new TupleList<string, ParameterValue>();
        }

        public HqlQuery(string hql)
            : this()
        {
            Hql = hql;
        }

        public HqlQuery Take(int rows)
        {
            MaxRows = rows;
            return this;
        }

        public HqlQuery Skip(int rows)
        {
            SkipRows = rows;
            return this;
        }

        public HqlQuery AddParameter(string key, object value)
        {
            Parameters.Add(key, new ParameterValue(value));
            return this;
        }

        public T GetParameter<T>(string key, T defaultValue = default(T))
        {
            var value = Parameters.FirstOrDefault(p => p.Item1 == key);
            if (value != null)
                return value.Item2.Value.ConvertTo<T>();
            return defaultValue;
        }
    }
}
