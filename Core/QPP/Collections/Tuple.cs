using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP.Collections
{
    [Serializable]
    [DataContract]
    public class Tuple<T1, T2>
    {
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        [DataMember]
        public T1 Item1 { get; private set; }
        [DataMember]
        public T2 Item2 { get; private set; }
    }
    [Serializable]
    [DataContract]
    public class Tuple<T1, T2, T3>
    {
        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
        [DataMember]
        public T1 Item1 { get; private set; }
        [DataMember]
        public T2 Item2 { get; private set; }
        [DataMember]
        public T3 Item3 { get; private set; }
    }
}
