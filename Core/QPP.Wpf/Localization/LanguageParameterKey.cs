using QPP.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Localization
{
    public class LanguageParameterKey : SysParameterKey<SysParameterCollectionValue<SysParameterValue<string, string, int>>>
    {
        static readonly SysParameterCollectionValue<SysParameterValue<string, string, int>> defaultValue = new SysParameterCollectionValue<SysParameterValue<string, string, int>>();
        
        public override string Name
        {
            get { return "系統語言"; }
        }

        public override string Category
        {
            get { return "基本參數"; }
        }

        public override string Description
        {
            get { return "系統支持的語言，語言代碼如：zh-cn,zh-tw,en-us"; }
        }

        [SysParameterDescription(Value1 = "語言名稱", Value2 = "語言代碼", Value3 = "顯示順序")]
        public override SysParameterCollectionValue<SysParameterValue<string, string, int>> DefaultValue
        {
            get { return defaultValue; }
        }
    }
}
