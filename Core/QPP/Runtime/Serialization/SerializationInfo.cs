using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QPP.Runtime.Serialization
{
    [Serializable]
    [DataContract]
    public class SerializationInfo
    {
        [DataMember]
        Dictionary<string, object> Values = new Dictionary<string, object>();

        public void AddValue(string name, bool value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, byte value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, char value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, DateTime value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, decimal value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, double value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, short value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, int value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, long value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, sbyte value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, float value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, ushort value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, uint value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, ulong value)
        {
            Values.Add(name, value);
        }
        public void AddValue(string name, string value)
        {
            Values.Add(name, value);
        }

        object GetValue(string name)
        {
            if(Values.ContainsKey(name))
                return Values[name];
            return null;
        }

        public bool? GetBoolean(string name)
        {
            return GetValue(name).ConvertTo<bool?>();
        }
        public byte? GetByte(string name)
        {
            return GetValue(name).ConvertTo<byte?>();
        }
        public char? GetChar(string name)
        {
            return GetValue(name).ConvertTo<char?>();
        }
        public DateTime? GetDateTime(string name)
        {
            return GetValue(name).ConvertTo<DateTime?>();
        }
        public decimal? GetDecimal(string name)
        {
            return GetValue(name).ConvertTo<decimal?>();
        }
        public double? GetDouble(string name)
        {
            return GetValue(name).ConvertTo<double?>();
        }
        public short? GetInt16(string name)
        {
            return GetValue(name).ConvertTo<short?>();
        }
        public int? GetInt32(string name)
        {
            return GetValue(name).ConvertTo<int?>();
        }
        public long? GetInt64(string name)
        {
            return GetValue(name).ConvertTo<long?>();
        }
        public sbyte? GetSByte(string name)
        {
            return GetValue(name).ConvertTo<sbyte?>();
        }
        public float? GetSingle(string name)
        {
            return GetValue(name).ConvertTo<float?>();
        }
        public string GetString(string name)
        {
            return GetValue(name).ConvertTo<string>();
        }
        public ushort? GetUInt16(string name)
        {
            return GetValue(name).ConvertTo<ushort?>();
        }
        public uint? GetUInt32(string name)
        {
            return GetValue(name).ConvertTo<uint?>();
        }
        public ulong? GetUInt64(string name)
        {
            return GetValue(name).ConvertTo<ulong?>();
        }
    }
}
