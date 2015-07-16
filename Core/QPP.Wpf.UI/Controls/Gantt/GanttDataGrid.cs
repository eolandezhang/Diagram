using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace QPP.Wpf.UI.Controls.Gantt
{
	public class GanttDataGrid : DataGrid
    {
        public event EventHandler<RowExpandedChangedEventArgs> RowExpandedChanged;

        protected internal ScrollViewer ScrollViewer { get; private set; }

        GanttNodeCollection m_Nodes;
        public GanttNodeCollection Nodes
        {
            get { return m_Nodes; }
            set { ItemsSource = m_Nodes = value; }
        }

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
            ScrollViewer = (ScrollViewer)GetTemplateChild("DG_ScrollViewer");            
		}

		protected internal void OnRowExpandedChanged(object sender, RowExpandedChangedEventArgs e)
        {
            if (e.Node == null)
                return;

            IGanttNode node = e.Node;

			int index = Nodes.IndexOf(node);

            if (node.Expanded)
				InsertChildNodes(node, ref index);
			else
				RemoveChildNodes(node);

            //Nodes.RaiseCollectionChanged();

			RaiseRowExpandedChanged(e);
		}
        
        protected void RaiseRowExpandedChanged(RowExpandedChangedEventArgs e)
        {
            if (RowExpandedChanged != null)
                RowExpandedChanged(this, e);
        }

		int InsertChildNodes(IGanttNode node, ref  int index)
		{
            foreach (IGanttNode childNode in node.Children)
			{
				Nodes.Insert(++index, childNode);

                if (childNode.Expanded && childNode.Children.Count > 0)
					InsertChildNodes(childNode, ref index);
			}
			return index;
		}

        void RemoveChildNodes(IGanttNode node)
		{
            foreach (IGanttNode childNode in node.Children)
			{
                if (childNode.Children.Count > 0)
					RemoveChildNodes(childNode);

				Nodes.Remove(childNode);
			}
		}
	}
}
