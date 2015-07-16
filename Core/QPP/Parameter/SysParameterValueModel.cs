using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QPP.Parameter
{
    [Serializable]
    [DataContract]
    public class SysParameterValueModel
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value1 { get; set; }
        [DataMember]
        public string Value2 { get; set; }
        [DataMember]
        public string Value3 { get; set; }
        [DataMember]
        public string Value4 { get; set; }
        [DataMember]
        public string Value5 { get; set; }
    }
}
