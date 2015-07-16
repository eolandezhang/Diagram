using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QPP.Parameter
{
    [Serializable]
    [DataContract]
    public class SysParameterKeyModel
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public IList<SysParameterValueModel> Values { get; set; }
        [DataMember]
        public string UpdateBy { get; set; }
    }
}
