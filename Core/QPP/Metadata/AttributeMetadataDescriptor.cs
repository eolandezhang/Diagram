using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Metadata
{
    public class AttributeMetadataDescriptor : IMetadataDescriptor
    {
        static Dictionary<Type, PresenterMetadata> m_ViewMetadata = new Dictionary<Type, PresenterMetadata>();

        public virtual PresenterMetadata GetMetadata(Type view)
        {
            if (m_ViewMetadata.ContainsKey(view))
                return m_ViewMetadata[view];

            var metadata = new PresenterMetadata();
            metadata.TypeName = view.FullName;
            var viewMetadataAttribute = Attribute.GetCustomAttribute(view, typeof(PresenterAttribute)) as PresenterAttribute;
            if (viewMetadataAttribute != null)
            {
                if (viewMetadataAttribute.Module != null)
                {
                    var type = viewMetadataAttribute.Module.GetType();
                    var applicationMetadataAttribute = Attribute.GetCustomAttribute(type, typeof(ModuleCatalogAttribute)) as ModuleCatalogAttribute;
                    if (applicationMetadataAttribute != null)
                        metadata.Module.AppType = applicationMetadataAttribute.AppType;
                    metadata.Module.Type = viewMetadataAttribute.Module;
                    metadata.Module.TypeName = type.FullName + "." + viewMetadataAttribute.Module;
                }
                metadata.Uri = viewMetadataAttribute.Uri;
                metadata.Properties["Icon"] = viewMetadataAttribute.Icon;
            }
            m_ViewMetadata.Add(view, metadata);
            return metadata;
        }
    }
}
