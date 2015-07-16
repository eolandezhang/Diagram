using System;
using System.ComponentModel;

namespace QPP.Wpf.UI.Controls.Gantt
{
	public enum DependencyType
    {
        /// <summary>
        /// 结束开始
        /// </summary>
        FS = 0,
        /// <summary>
        /// 结束结束
        /// </summary>
        FF = 1,
        /// <summary>
        /// 开始结束
        /// </summary>
        SF = 2,
        /// <summary>
        /// 开始开始
        /// </summary>
        SS = 3,
	}

    public class GanttDependency : INotifyPropertyChanged
	{
        //public event EventHandler TypeChanged;
        //public event EventHandler PredecessorChanged;
        //public event EventHandler SuccessorChanged;             
		private DependencyType _Type;
        private IGanttNode _Predecessor;
        private IGanttNode _Successor;
        public IGanttNode Predecessor
        {
            get
            {
                return _Predecessor;
            }
            set
            {
                if (_Predecessor!=value)
                {
                    _Predecessor = value;
                    if (PropertyChanged!=null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Predecessor"));
                }
            }
        }
        public IGanttNode Successor
        {
            get
            {
                return _Successor;
            }
            set
            {
                if (_Successor!=value)
                {
                    _Successor = value;
                    if (PropertyChanged!=null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Successor"));
                }
            }
        }
		public DependencyType Type
		{
			get
			{
				return _Type;
			}
			set
			{
				if (_Type != value)
				{
					_Type = value;
                    if (PropertyChanged!=null)                 
                        PropertyChanged(this, new PropertyChangedEventArgs("Type"));
				}
			}
		}

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
