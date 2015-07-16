using System;
using System.Collections.Generic;

namespace QPP.Validation
{
    [Serializable]
    public class ErrorInfoCollection : ICollection<ErrorInfo>
    {
        List<ErrorInfo> list = new List<ErrorInfo>();

        public ErrorInfoCollection() { }

        public void Add(ErrorInfo error)
        {
            ErrorInfo e = Get(error.FieldName, error.SheetName, error.RowNum);
            if (e != null)
                e.Errors.AddRange(error.Errors);
            else
                list.Add(error);
        }

        public void AddRange(IEnumerable<ErrorInfo> errors)
        {
            foreach (ErrorInfo e in errors)
                Add(e);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public void Clear()
        {
            list.Clear();
        }

        public ErrorInfo this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public ErrorInfo Get(List<string> fieldName, string sheetName, int rowNum)
        {
            foreach (ErrorInfo error in list)
                if (error.FieldName.Count == fieldName.Count
                    && error.FieldName.TrueForAll(p => fieldName.Contains(p))
                    && error.SheetName == sheetName && error.RowNum == rowNum)
                    return error;
            return null;
        }

        public bool ContainKey(string fieldName)
        {
            foreach (ErrorInfo error in list)
                if (error.FieldName.Equals(fieldName))
                    return true;
            return false;
        }

        public IEnumerator<ErrorInfo> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public bool Contains(ErrorInfo item)
        {
            return false;
        }

        public void CopyTo(ErrorInfo[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ErrorInfo item)
        {
            foreach (ErrorInfo error in list)
                if (error.FieldName.Equals(item.FieldName))
                {
                    foreach (var e in item.Errors)
                        error.Errors.Remove(e);
                    return true;
                }
            return false;
        }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    [Serializable]
    public class ErrorInfo
    {
        public ErrorInfo()
        {
            Errors = new ErrorTextCollection();
            FieldName = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="rowNum">行号</param>
        /// <param name="error">错误文本</param>
        public ErrorInfo(string fieldName, ErrorText error, int rowNum = int.MinValue, string sheetName = "")
            : this()
        {
            FieldName.Add(fieldName);
            RowNum = rowNum;
            SheetName = sheetName;
            Errors.Add(error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="rowNum">行号</param>
        /// <param name="error">错误文本</param>
        public ErrorInfo(IEnumerable<string> fieldName, ErrorText error, int rowNum = int.MinValue, string sheetName = "")
            : this()
        {
            FieldName.AddRange(fieldName);
            RowNum = rowNum;
            SheetName = sheetName;
            Errors.Add(error);
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public List<string> FieldName { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int RowNum { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 错误文本集合
        /// </summary>
        public ErrorTextCollection Errors { get; set; }
    }
}
