using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QPP.Security
{
    [Serializable]
    [DataContract]
    public class UserIdentity : IIdentity
    {
        public UserIdentity(string id, string name)
        {
            Permission = new Dictionary<string, List<string>>();
            Id = id;
            UserName = name;
        }
        [DataMember]
        public string Id { get; private set; }
        [DataMember]
        public string UserName { get; private set; }
        [DataMember]
        public Dictionary<string, List<string>> Permission
        {
            get;
            private set;
        }
    }
}
