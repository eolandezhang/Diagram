using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.Wpf.UI.Controls.AvalonDock.Layout;
using QPP.Wpf.UI.Controls.AvalonDock;
using System.Windows;
using QPP.Layout;

namespace QPP.Wpf.Layout
{
    public static class Extand
    {
        /// <summary>
        /// 把DockingDocument添加到佈局
        /// </summary>
        /// <param name="dockManager"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static LayoutDocument AddToLayout(this DockingManager dockManager, DockingDocument content)
        {
            var doc = new LayoutDocument();
            doc.Content = content;
            content.PropertyChanged += (s, e) =>
            {
                if (e.Property.Name == "Title")
                    doc.Title = content.Title;
                else if (e.Property.Name == "IconSource")
                    doc.IconSource = content.IconSource;
            };
            doc.Closing += (s, e) =>
            {
                content.OnClosing(e);
            };
            doc.Closed += (s, e) =>
            {
                content.OnClosed(e);
            };
            doc.IconSource = content.IconSource;
            doc.Title = content.Title;
            doc.FloatingTop = content.FloatingTop;
            doc.FloatingLeft = content.FloatingLeft;
            doc.FloatingHeight = content.FloatingHeight;
            doc.FloatingWidth = content.FloatingWidth;

            var firstDocumentPane = dockManager.Layout.Descendents()
                .OfType<LayoutDocumentPane>().FirstOrDefault();
            firstDocumentPane.Children.Insert(0, doc);
            doc.IsActive = true;
            return doc;
        }

        /// <summary>
        /// 把DockingAnchorable添加到佈局
        /// </summary>
        /// <param name="dockManager"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static LayoutAnchorable AddToLayout(this DockingManager dockManager, DockingAnchorable content)
        {
            var doc = new LayoutAnchorable();
            doc.Content = content;
            content.PropertyChanged += (s, e) =>
            {
                if (e.Property.Name == "Title")
                    doc.Title = content.Title;
                else if (e.Property.Name == "IconSource")
                    doc.IconSource = content.IconSource;
            };
            doc.Closing += (s, e) =>
            {
                content.OnClosing(e);
            };
            doc.Closed += (s, e) =>
            {
                content.OnClosed(e);
            };
            doc.IconSource = content.IconSource;
            doc.Title = content.Title;
            doc.AutoHideHeight = content.AutoHideHeight;
            doc.AutoHideWidth = content.AutoHideWidth;
            doc.AutoHideMinHeight = content.MinHeight;
            doc.AutoHideMinWidth = content.MinWidth;
            doc.FloatingTop = content.FloatingTop;
            doc.FloatingLeft = content.FloatingLeft;
            doc.FloatingHeight = content.FloatingHeight;
            doc.FloatingWidth = content.FloatingWidth;

            if (content.DockArea == DockAreas.DockLeft)
            {
                doc.AddToLayout(dockManager, AnchorableShowStrategy.Left);
            }
            else if (content.DockArea == DockAreas.DockRight)
            {
                doc.AddToLayout(dockManager, AnchorableShowStrategy.Right);
            }
            else if (content.DockArea == DockAreas.DockTop)
            {
                doc.AddToLayout(dockManager, AnchorableShowStrategy.Top);
            }
            else if (content.DockArea == DockAreas.DockBottom)
            {
                doc.AddToLayout(dockManager, AnchorableShowStrategy.Bottom);
            }
            else
            {
                var firstDocumentPane = dockManager.Layout.Descendents()
                    .OfType<LayoutDocumentPane>().FirstOrDefault();
                firstDocumentPane.Children.Insert(0, doc);
            }
            doc.IsActive = true;
            return doc;
        }
        
        public static LayoutContent AddToLayout(this DockingManager dockManager, IDockingContent docking)
        {
            if (docking is DockingAnchorable)
                return AddToLayout(dockManager, (DockingAnchorable)docking);
            if (docking is DockingDocument)
                return AddToLayout(dockManager, (DockingDocument)docking);
            throw new InvalidCastException();
        }
    }
}
