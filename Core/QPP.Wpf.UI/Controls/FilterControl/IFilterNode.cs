using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using QPP.ComponentModel;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public interface IFilterNode 
    {
         string Name { get; set; }

         string FieldName { get; set; }

         ActionType Action { get; set; }

         object Value { get; set; }

         Scope Scope { get; set; }

         ObservableCollection<IFilterNode> Children { get; set; }

         IFilterNode Parent { get; set; }

         int Level { get; set; }

         bool IsGroup { get; set; }

         int HorizontalOffset { get;}

         string ChildrenRelation { get; set; }

         TypeCode Type { get; set; }

         FilterDataTemplate FilterDataTemplate { get; set; }
        
    }
}
