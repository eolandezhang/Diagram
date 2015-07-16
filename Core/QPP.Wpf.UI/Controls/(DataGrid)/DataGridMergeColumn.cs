using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls
{
    public class DataGridMergeColumn : DataGridTextColumn
    {
        protected override System.Windows.FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return base.GenerateEditingElement(cell, dataItem);
        }

        protected override System.Windows.FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return base.GenerateElement(cell, dataItem);
        }
    }
}
