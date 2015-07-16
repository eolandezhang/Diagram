using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ComponentModel
{
    public class DataArgs
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();

        public static DataArgs Create(string key, object value)
        {
            return new DataArgs().Set(key, value);
        }

        /// <summary>
        /// 從命令行參數中查找指定的參數，找不到時返回null。
        /// 命令行格式示例：qpmrpx:bpms?username=ericleung&companyid=qp
        /// </summary>
        public static DataArgs CommandLineArgs { get; private set; }

        static DataArgs()
        {
            CommandLineArgs = new DataArgs();
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.IndexOf("?") > 0)
                {
                    var query = arg.Substring(arg.IndexOf('?') + 1);
                    foreach (var a in Parse(query).parameters)
                        CommandLineArgs.Set(a.Key, a.Value);
                }
            }
        }

        /// <summary>
        /// 設置鍵值
        /// </summary>
        /// <param name="key">不區分大小寫</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataArgs Set(string key, object value)
        {
            var k = key.ToLower();
            parameters[k] = value;
            return this;
        }

        /// <summary>
        /// 獲取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">不區分大小寫</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T Get<T>(string key, T defaultValue = default(T))
        {
            var k = key.ToLower();
            if (parameters.ContainsKey(k))
                return parameters[k].ConvertTo<T>();
            return defaultValue;
        }

        /// <summary>
        /// 合併參數，如果已經存在，則覆蓋
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataArgs Combine(DataArgs args)
        {
            foreach (var p in args.parameters)
                parameters[p.Key] = p.Value;
            return this;
        }

        public static DataArgs Parse(string queryString)
        {
            var query = queryString;
            if (query.IndexOf("?") > 0)
                query = query.Substring(query.IndexOf('?') + 1);
            DataArgs result = new DataArgs();
            foreach (var p in query.Split('&'))
            {
                if (p.IsNullOrWhiteSpace()) continue;
                var key = p.Split('=');
                if (key.Length > 1)
                {
                    result.Set(key[0], key[1]);
                }
            }
            return result;
        }

        public override string ToString()
        {
            var result = "";
            foreach (var p in parameters)
                result += p.Key + "=" + p.Value + "&";
            return result.TrimEnd('&');
        }
    }
}
