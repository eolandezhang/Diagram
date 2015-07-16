using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace QPP.Collections
{
    /// <summary>
    /// 有状态的集合，记录删除的列表
    /// </summary>
    public interface IStatefulCollection : IList
    {
        IList Deleted { get; }
        void ResumePropertyChanged();
        void SuppressPropertyChanged();
        void RaiseCollectionChanged();
    }
}
