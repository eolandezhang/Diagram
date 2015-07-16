using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace QPP.Wpf.UI.Controls.XGantt
{
	public class GanttDependenciesPresenter : Canvas
	{
		#region Properties
		protected internal GanttPanel ParentPanel { get; set; }
		protected bool ItemsValid { get; set; }
		#endregion

		#region Constructors and overrides
		public GanttDependenciesPresenter()
		{
			this.UseLayoutRounding = false;
            this.Loaded += GanttDependenciesPresenter_Loaded;

            DependencyLinker = new Line();
            DependencyLinker.Stroke = new SolidColorBrush(Colors.Black);
            DependencyLinker.StrokeDashOffset = 3;
            DependencyLinker.StrokeThickness = 1;
            DependencyLinker.IsHitTestVisible = false;
            DependencyLinker.Visibility = System.Windows.Visibility.Collapsed;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
            foreach (GanttDependencyItem item in Children.OfType<GanttDependencyItem>())
			{			
			    item.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			}

			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
            foreach (GanttDependencyItem item in Children.OfType<GanttDependencyItem>())
            {
                item.Measure(availableSize);
            }

			return base.MeasureOverride(availableSize);
		}
		#endregion

		#region Event handling functions
		private void GanttDependenciesPresenter_Loaded(object sender, RoutedEventArgs e)
		{
            if (ParentPanel.Dependencies == null) return;
			ParentPanel.Dependencies.CollectionChanged += Dependencies_CollectionChanged;
		}
		private void Dependencies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Invalidate();
		}
		#endregion

		#region Internal functions
		protected internal void Invalidate()
		{
			Children.Clear();
            if (ParentPanel.Dependencies == null) return;
			foreach (GanttDependency gd in ParentPanel.Dependencies)
			{                
                bool result = IsAddDependenceLinker(gd);   
                if (result)
                    this.Children.Add(new GanttDependencyItem { Dependency = gd, ParentPresenter = this });
                //if ((gd.Predecessor.Parent == null || gd.Predecessor.Parent.Expanded)
                //   && (gd.Successor.Parent == null || gd.Successor.Parent.Expanded)
                //    )
                //    this.Children.Add(new GanttDependencyItem { Dependency = gd, ParentPresenter = this });
			}
            Children.Add(DependencyLinker);
		}

        //判断是否符合增加的条件
        private bool IsAddDependenceLinker(GanttDependency gd)
        {
            if (gd.Predecessor.Node.Parent == null && gd.Successor.Node.Parent == null)
                return true;
            if (gd.Predecessor.Node.Parent == null && gd.Successor.Node.Parent != null)
            {
                if (gd.Successor.Node.Parent.Expanded)
                    //后置是否存在祖父折叠的，存在的话，就会不加线条
                    return IsExistParent(gd.Successor.Node.Parent);
            }
            if (gd.Predecessor.Node.Parent != null && gd.Successor.Node.Parent == null)
            {
                if (gd.Predecessor.Node.Parent.Expanded)
                    //前置是否存在祖父折叠的，存在的话，就会不加线条
                    return IsExistParent(gd.Predecessor.Node.Parent);
            }
            if (gd.Predecessor.Node.Parent != null && gd.Successor.Node.Parent != null)
            {
                //前置或者后置是否存在祖父折叠的，存在的话，就会不加线条
                if (gd.Successor.Node.Parent.Expanded && !gd.Predecessor.Node.Parent.Expanded)
                    return IsExistParent(gd.Predecessor.Node.Parent);
                if (gd.Predecessor.Node.Parent.Expanded && !gd.Successor.Node.Parent.Expanded)
                    return IsExistParent(gd.Successor.Node.Parent);
                if (gd.Successor.Node.Parent.Expanded && gd.Predecessor.Node.Parent.Expanded)
                    return IsExistParent(gd.Successor.Node.Parent) && IsExistParent(gd.Predecessor.Node.Parent);
            }
            return false;
        }

        //是否存在祖父折叠的
        bool IsExistParent(IGanttNode node)
        {
            if (!node.Expanded)
                return false;
            if (node.Parent != null)
                return IsExistParent(node.Parent);
            return true;
        }

        internal Line DependencyLinker
        {
            get;
            private set;
        }

		#endregion
	}
}
