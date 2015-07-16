using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 用於標記業務對象中的字段，此字段可讓用戶快速區分業務對象
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CaptionAttribute : Attribute
    {
    }
}
