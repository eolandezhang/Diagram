using QPP.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public class Module : IModule
    {
        public ModuleMetadata Metadata { get; set; }

        public bool CanSerialize { get; set; }

        public void Serialize(Runtime.Serialization.SerializationInfo info)
        {
            OnSerialize(info);
        }

        public void Deserialize(Runtime.Serialization.SerializationInfo info)
        {
            OnDeserialize(info);
        }

        protected virtual void OnSerialize(Runtime.Serialization.SerializationInfo info)
        {

        }

        protected virtual void OnDeserialize(Runtime.Serialization.SerializationInfo info)
        {

        }
    }
}
