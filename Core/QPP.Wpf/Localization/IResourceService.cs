﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Localization
{
    public interface IResourceService
    {
        IList<IResourceModel> GetResources();
    }
}
