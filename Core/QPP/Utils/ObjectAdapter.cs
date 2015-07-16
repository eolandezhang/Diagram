using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Data;

namespace QPP.Utils
{
    public class ObjectAdapter
    {
        public static void FillModel<T>(IOrderedDictionary values, T model)
        {
            Type type = model.GetType();
            foreach (string k in values.Keys)
            {
                object obj = values[k];
                if (obj != null && obj != DBNull.Value)
                {
                    PropertyInfo info = type.GetProperty(k);
                    Type t = info.PropertyType;
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                        t = t.GetGenericArguments()[0];
                    if (obj.GetType() == t || t.IsEnum)
                        info.SetValue(model, obj, null);
                    else if (t == typeof(Guid))
                        info.SetValue(model, new Guid(obj.ToString()), null);
                    else
                        info.SetValue(model, Convert.ChangeType(obj, t), null);
                }
            }
        }

        public static T ToModel<T>(IOrderedDictionary values) where T : new()
        {
            T model = new T();
            Type type = model.GetType();
            foreach (string k in values.Keys)
            {
                object obj = values[k];
                if (obj != null && obj != DBNull.Value)
                {
                    PropertyInfo info = type.GetProperty(k);
                    Type t = info.PropertyType;
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                        t = t.GetGenericArguments()[0];
                    if (obj.GetType() == t || t.IsEnum)
                        info.SetValue(model, obj, null);
                    else if (t == typeof(Guid))
                        info.SetValue(model, new Guid(obj.ToString()), null);
                    else
                        info.SetValue(model, Convert.ChangeType(obj, t), null);
                }
            }
            return model;
        }

        /// <summary> 
        /// DataSet的第index个表转换为实体类
        /// </summary> 
        public static List<T> ToModel<T>(DataSet ds, int index = 0) where T : new()
        {
            return ToModel<T>(ds.Tables[index]);
        }

        /// <summary> 
        /// DataTable转换成实体列表
        /// </summary> 
        public static List<T> ToModel<T>(DataTable dt) where T : new()
        {
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T model = ToModel<T>(dr);
                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary> 
        /// DataRow转换成实体类
        /// </summary> 
        public static T ToModel<T>(DataRow dr) where T : new()
        {
            T model = new T();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                if (propertyInfo != null && dr[i] != DBNull.Value)
                    propertyInfo.SetValue(model, Convert.ChangeType(dr[i], propertyInfo.PropertyType), null);
            }
            return model;
        }

        /// <summary>
        /// 实体类转换成DataSet
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataSet ToDataSet<T>(IList<T> modelList)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(ToDataTable(modelList));
            return ds;
        }

        /// <summary>
        /// 实体类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IList<T> modelList)
        {
            DataTable dt = CreateDataTable<T>();
            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                FillDataRow(model, dataRow);
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        public static List<string> GetPropertyNames<T>()
        {
            List<string> list = new List<string>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                list.Add(propertyInfo.Name);
            }
            return list;
        }

        public static DataRow FillDataRow<T>(T model, DataRow row)
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                object obj = propertyInfo.GetValue(model, null);
                row[propertyInfo.Name] = obj ?? DBNull.Value;
            }
            return row;
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        public static DataTable CreateDataTable<T>()
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo p in typeof(T).GetProperties())
            {
                System.Type type = p.PropertyType;
                if (p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = p.PropertyType.GetGenericArguments()[0];
                }

                DataColumn column = new DataColumn(p.Name, type);
                column.AllowDBNull = true;
                dataTable.Columns.Add(column);
            }
            return dataTable;
        }
    }
}
