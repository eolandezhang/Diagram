using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace QPP.Utils
{
    public static class ObjectHelper
    {
        public static object Clone(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return null;
            if (obj is ICloneable)
                return ((ICloneable)obj).Clone();
            return DoBinaryClone(obj);
        }

        public static object BinaryClone(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return null;
            return DoBinaryClone(obj);
        }

        static object DoBinaryClone(object obj)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(buffer, obj);
                buffer.Position = 0;
                object temp = formatter.Deserialize(buffer);
                return temp;
            }
        }

        public static byte[] Serialize(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static object Deserialize(byte[] bytes)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return formatter.Deserialize(stream);
            }
        }
    }
}
