using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP
{
    [DataContract]
    public class PageData<T>
    {
        /// <summary>
        /// 记录总数，指符合条件的记录总数
        /// </summary>
        [DataMember]
        public int Total { get; set; }

        /// <summary>
        /// 一页内的业务数据
        /// </summary>
        [DataMember]
        public IList<T> Data { get; set; }

        public PageData<U> Cast<U>()
        {
            var result = new PageData<U>();
            result.Total = Total;
            result.Data = Data.Cast<U>().ToList();
            return result;
        }
    }
}
