using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.TreeEditor
{
    public class VerticalTreeManager : DiagramManager
    {
        public VerticalTreeManager(DiagramControl diagramControl) : base(diagramControl)
        {
        }
        //public void Draw()
        //{
        //    //_diagramControl.AddToMessage("载入数据源", GetTime(() =>
        //    //{
        //    base.ClearCanvas();
        //    if (base.CheckDesignerItemsIsNullOrEmpty()) return;
        //    GetRootItems().ForEach(root => { base.DrawDesignerItems(root); });
        //    Arrange();
        //    base.SelectFirstRoot();
        //    //}));

        //}
    }
}
