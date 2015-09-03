using System.Collections.Generic;
using System.Windows;
using System;

namespace QPP.Wpf.UI.TreeEditor
{
    public interface IDiagramManager
    {
        void Arrange();
        //void AddNewArrange(DesignerItem newItem);
        void DeleteArrange(DesignerItem delItem);





        void Edit(DesignerItem item, string key = "");
        void Edit(string key = "");
        void DeleteItem(string id);






        DesignerItem GetDesignerItemById(string id);
        List<DesignerItem> GetSelectedItemsAndAllSubItems();






        void Draw();
        void DrawChild /*创建非根节点时，同时创建与父节点之间的连线*/(DesignerItem parent, DesignerItem childItem, List<DesignerItem> list);
        void DrawRoot /*创建根节点*/(DesignerItem item);






        void CollapseAll /*折叠所有，除了根节点*/();
        void ExpandAll /*展开所有*/();
        void ExpandSelectedItem();
        void CollapseSelectedItem();
        void HideOrExpandChildItems /*展开折叠*/(DesignerItem item);





        DesignerItem CreateItemShadow /*拖动时创建的影子*/(DesignerItem item);
        DesignerItem ChangeParent(Point position, DesignerItem designerItem, List<DesignerItem> selectedItemsAllSubItems);
        void CreateHelperConnection(DesignerItem newParent, DesignerItem dragItem);
        void AfterChangeParent(DesignerItem designerItem, DesignerItem newParent, Point newPosition,
            List<DesignerItem> selectedItemsAllSubItems);
        void MoveUpAndDown(DesignerItem parent, DesignerItem selectedItem, DesignerItem root);




        void SelectUpDown(bool selectUp);
        void SelectRightLeft(bool selectRight);





        void Scroll(DesignerItem designerItem);
        void SetSelectItem(DesignerItem designerItem);




        void SavePosition();

        string GetTime(Action action);
    }
}
