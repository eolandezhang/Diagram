using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace QPP.Wpf.UI.Controls.Gantt
{
    public class GanttItemShadow : Control
    {
        #region Constants
        public const double HANDLE_MARGIN = 5.0;
        #endregion

        #region Constructors and overrides

        static GanttItemShadow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GanttItemShadow), new FrameworkPropertyMetadata(typeof(GanttItemShadow)));
        }

        public GanttItemShadow()
        {
            //StartDate = DateTime.Now;
            //EndDate = DateTime.Now;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime EndDate
        {
            get;
            set;
        }

        #endregion
    }
}
