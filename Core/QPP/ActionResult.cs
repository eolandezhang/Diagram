using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP
{
    [DataContract]
    public class ActionResult
    {
        public ActionResult() { }

        public ActionResult(string message)
        {
            Message = message;
        }

        public ActionResult(string message, bool success)
        {
            Message = message;
            Success = success;
        }

        protected bool m_Success = true;
        /// <summary>
        /// 是否能成功返回数据
        /// </summary>
        [DataMember]
        public bool Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        /// <summary>
        /// 附加信息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    [KnownType("GetKnownTypes")]
    [Newtonsoft.Json.JsonObject()]
    public class ActionResult<T> : ActionResult
    {
        static Type[] GetKnownTypes()
        {
            return new Type[] { typeof(DataResult<T>), typeof(ListResult<T>) };
        }
    }

    [DataContract]
    public class DataResult<T> : ActionResult<T>
    {
        public DataResult() { }

        public DataResult(T obj)
        {
            Data = obj;
        }

        /// <summary>
        /// 一页内的业务数据
        /// </summary>
        [DataMember]
        public T Data { get; set; }
    }

    [DataContract]
    public class ListResult<T> : ActionResult<T>
    {
        public ListResult() { }

        public ListResult(IList<T> lst)
        {
            Total = lst.Count;
            Data = lst;
        }

        public ListResult(PageData<T> lst)
        {
            Total = lst.Total;
            Data = lst.Data;
        }

        /// <summary>
        /// 记录总数，指符合条件的记录总数
        /// </summary>
        [DataMember]
        public long Total { get; set; }

        /// <summary>
        /// 一页内的业务数据
        /// </summary>
        [DataMember]
        public IList<T> Data { get; set; }
    }
}