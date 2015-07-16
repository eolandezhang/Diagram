using System;
using System.Runtime.Serialization;

namespace QPP
{
    [Serializable]
    [DataContract]
    public class CriteriaQuery
    {
        public static CriteriaQuery Empty = new CriteriaQuery();

        public CriteriaQuery()
        {
            MaxRows = -1;
        }

        [DataMember]
        public string Criteria { get; set; }

        [DataMember]
        public int SkipRows { get; set; }

        [DataMember]
        public int MaxRows { get; set; }

        [DataMember]
        public string Sort { get; set; }
    }
}
