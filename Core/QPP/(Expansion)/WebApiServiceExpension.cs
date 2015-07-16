using QPP.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP
{
    public static class WebApiServiceExpension
    {

        /// <summary>
        /// HttpMethod.Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T Fetch<T>(this IWebApiService service, string url)
        {
            return service.Get<DataResult<T>>(url).Data;
        }

        /// <summary>
        /// HttpMethod.Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T Fetch<T>(this IWebApiService service, string url, object data)
        {
            return service.Post<DataResult<T>>(url, data).Data;
        }

        /// <summary>
        /// HttpMethod.Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IList<T> FetchList<T>(this IWebApiService service, string url, object data)
        {
            return service.Post<ListResult<T>>(url, data).Data;
        }
        /// <summary>
        /// HttpMethod.Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IList<T> FetchList<T>(this IWebApiService service, string url)
        {
            return service.Get<ListResult<T>>(url).Data;
        }

        /// <summary>
        /// HttpMethod.Put
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        public static void Update(this IWebApiService service, string url, object obj)
        {
            service.Put(url, obj);
        }

        /// <summary>
        /// HttpMethod.Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Add(this IWebApiService service, string url, object obj)
        {
            return service.Post<DataResult<object>>(url, obj).Data;
        }

        /// <summary>
        /// HttpMethod.Delete
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static void Delete(this IWebApiService service, string url)
        {
            service.Delete(url);
        }
    }
}
