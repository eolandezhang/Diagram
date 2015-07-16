using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using QPP.Data;
using System.Linq.Expressions;
using QPP.Filtering;
using QPP.Utils;
using QPP.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml.Linq;
using QPP.Api;
using Newtonsoft.Json;

namespace QPP
{
    public static class Expension
    {
        //public static Criteria<T> ToCriteria<T>(this T op, Expression<Func<T, bool>> expr)
        //{
        //    return Criteria.Create<T>(expr);
        //}

        /// <summary>
        /// if object is null reference, will return string.Empty. else call object.ToString() 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>string</returns>
        public static string ToSafeString(this object obj)
        {
            if (object.Equals(obj, null))
                return string.Empty;
            return obj.ToString();
        }
        /// <summary>
        /// Convert object to specified Type
        /// </summary>
        /// <typeparam name="T">desired type</typeparam>
        /// <param name="obj"></param>
        /// <param name="defaultValue">default value when convert failed</param>
        /// <param name="ignoreException">true will ignore exception when convert failed. default is true</param>
        /// <returns>object of desired type</returns>
        public static T ConvertTo<T>(this object obj, T defaultValue, bool ignoreException = true)
        {
            if (ignoreException)
            {
                try
                {
                    return obj.ConvertTo<T>();
                }
                catch
                {
                    return defaultValue;
                }
            }
            return obj.ConvertTo<T>();
        }
        /// <summary>
        /// Convert object to specified Type
        /// </summary>
        /// <typeparam name="T">desired type</typeparam>
        /// <param name="obj">original object</param>
        /// <returns>object of desired type</returns>
        public static T ConvertTo<T>(this object obj)
        {
            if (obj != null)
            {
                if (obj is T)
                    return (T)obj;

                var sourceType = obj.GetType();
                var targetType = typeof(T);

                if (targetType.IsEnum)
                    return (T)Enum.Parse(targetType, obj.ToString(), true);


                if (sourceType.IsAssignableFrom(typeof(IConvertible)) &&
                    sourceType.IsAssignableFrom(typeof(IConvertible)))
                    return (T)Convert.ChangeType(obj, targetType);

                var converter = TypeDescriptor.GetConverter(obj);
                if (converter != null && converter.CanConvertTo(targetType))
                    return (T)converter.ConvertTo(obj, targetType);

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null && converter.CanConvertFrom(sourceType))
                    return (T)converter.ConvertFrom(obj);

                if (targetType.IsValueType)
                {
                    try
                    {
                        var t = targetType;
                        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            t = targetType.GetGenericArguments()[0];
                        var typecode = Type.GetTypeCode(t);
                        return (T)Convert.ChangeType(obj, typecode);
                    }
                    catch { }
                }

                throw new InvalidOperationException("Can't convert from type {0} to type {1}."
                            .FormatArgs(sourceType.Name, targetType.Name));
            }
            else
            {
                Type t = typeof(T);
                if (t == typeof(string))
                    return default(T);
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return default(T);
                throw new ArgumentNullException("obj");
            }
        }

        public static Uri RemoveQueryString(this Uri uri, string keyToRemove)
        {
            string url = uri.OriginalString;
            //if first parameter, leave ?, take away trailing &         
            string pattern = @"\?" + keyToRemove + "[^&]*&?";
            url = System.Text.RegularExpressions.Regex.Replace(url, pattern, "?");
            //if subsequent parameter, take away leading &         
            pattern = "&" + keyToRemove + "[^&]*";
            url = System.Text.RegularExpressions.Regex.Replace(url, pattern, "");
            if (uri.IsAbsoluteUri)
                return new Uri(url);
            return new Uri(url, UriKind.Relative);
        }

        public static Uri AddQueryString(this Uri uri, string key, string value)
        {
            string url = uri.RemoveQueryString(key).OriginalString;
            if (url.IndexOf('?') == 0)
                url += "?";
            if (uri.Query.Length > 1)
                url += "&";
            if (uri.IsAbsoluteUri)
                return new Uri(url + key + "=" + value);
            return new Uri(url + key + "=" + value, UriKind.Relative);
        }

        public static T ToModel<T>(this IOrderedDictionary values) where T : new()
        {
            return ObjectAdapter.ToModel<T>(values);
        }

        public static void FillModel<T>(this IOrderedDictionary values, T model)
        {
            ObjectAdapter.FillModel<T>(values, model);
        }

        public static void Save(this Stream stream, string filePath)
        {
            FileStream writeStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            stream.Position = 0;
            int bytesRead = stream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = stream.Read(buffer, 0, Length);
            }
            stream.Close();
            writeStream.Close();
        }

        public static string GetElementValue(this XElement element, string name)
        {
            if (element == null)
                return null;
            var node = element.Element(name);
            if (node == null)
                return null;
            return node.Value;
        }
    }
}
