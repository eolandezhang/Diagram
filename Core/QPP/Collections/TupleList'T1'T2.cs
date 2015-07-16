using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Collections
{
    [Serializable]
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public void Add(T1 item1, T2 item2)
        {
            Add(new Tuple<T1, T2>(item1, item2));
        }
    }
}
