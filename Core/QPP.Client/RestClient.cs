using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QPP.Validation;
using QPP.ServiceLocation;
using Common.Logging;
using QPP.Security;

namespace QPP.Client
{
    public class RestClient
    {
        /// <summary>
        /// BaseAddress配置Key為RestServiceUri
        /// </summary>
        public static readonly RestClient Default = new RestClient(ConfigurationManager.AppSettings["RestServiceUri"], ConfigurationManager.AppSettings["RestRequestTimeout"].ConvertTo<int>(600));
        public static string AuthCookieName = ".ASPXAUTH";
        public string BaseAddress { get; set; }
        public int Timeout { get; set; }
        ILog m_Log = LogManager.GetLogger(typeof(RestClient));
        IAuthenticationContext m_AuthContext;

        IAuthenticationContext AuthContext
        {
            get
            {
                if (m_AuthContext == null)
                {
                    if (RuntimeContext.Current.Locator.ContainsObject("AuthenticationContext"))
                        m_AuthContext = RuntimeContext.Current.Locator.GetObject<IAuthenticationContext>("AuthenticationContext");

                    if (m_AuthContext == null)
                        m_AuthContext = new DomainAuthenticationContext();
                }
                return m_AuthContext;
            }
        }

        public RestClient SetAuthenticationTicket(IAuthenticationContext context)
        {
            m_AuthContext = context;
            return this;
        }

        public RestClient(string baseAddress = null, int timeout = 600)
        {
            BaseAddress = baseAddress;
            Timeout = timeout;
        }
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SendData(string url, HttpMethod method, string data = null)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (data != null && (method != HttpMethod.Post && method != HttpMethod.Put))
                throw new ArgumentException("HttpMethod can only be Post or Put when data is not null");

            var m_Handler = new HttpClientHandler
            {
                UseCookies = true,
                UseDefaultCredentials = true,
                CookieContainer = new CookieContainer()
            };

            var m_HttpClient = new HttpClient(m_Handler);

            if (!BaseAddress.EndsWith("\\") && !BaseAddress.EndsWith("/"))
                BaseAddress += "/";
            m_HttpClient.BaseAddress = new Uri(BaseAddress);
            m_HttpClient.Timeout = TimeSpan.FromSeconds(Timeout);

            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 0.99));
            if (data != null)
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");

            var ticket = AuthContext.GetTicket();
            if (ticket.IsNotEmpty())
            {
                if (AuthContext.AuthenticationMode == Security.AuthenticationMode.Basic)
                    request.Headers.Add("Authorization", "Basic " + ticket);
                else
                    request.Headers.Add("Authorization", "Ticket " + ticket);
            }

            var response = m_HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
            if (response.IsSuccessStatusCode)
            {
                if (AuthContext.AuthenticationMode == Security.AuthenticationMode.Ticket)
                {
                    var cookies = m_Handler.CookieContainer.GetCookies(m_HttpClient.BaseAddress);
                    var authCookie = cookies.OfType<Cookie>().FirstOrDefault(p => p.Name == AuthCookieName);
                    if (authCookie != null)
                    {
                        //m_Log.Info("authCookie from {0}{1}:{2}".FormatArgs(m_HttpClient.BaseAddress, url, authCookie.Value));
                        AuthContext.SetTicket(authCookie.Value);
                    }
                }
                string responseText = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ActionResult>(responseText);
                if (result.Success)
                    return responseText;
                if (result.Message.StartsWith("invalid|"))
                {
                    throw JsonConvert.DeserializeObject<ValidationException>(result.Message.Substring(8));
                }
                else
                    throw new Exception(result.Message);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    m_Log.Warn("BaseAddress:{0},Url:{1},Ticket:{2}".FormatArgs(m_HttpClient.BaseAddress, url, ticket));
                    throw new System.Security.Authentication.AuthenticationException(response.ReasonPhrase);
                }
                m_Log.Error("BaseAddress:{0},Url:{1},Ticket:{2}".FormatArgs(m_HttpClient.BaseAddress, url, ticket));
                string text = null;
                try
                {
                    text = response.Content.ReadAsStringAsync().Result;
                }
                catch { }
                var msg = string.Format("{0} ({1})\r\nUrl:{2}\r\nMethod:{3}\r\nRespondText:{4}", (int)response.StatusCode, response.ReasonPhrase, System.IO.Path.Combine(m_HttpClient.BaseAddress.OriginalString, url), method, text);
                throw new HttpRequestException(msg);
            }
        }

        /// <summary>
        /// HttpMethod.Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public T Fetch<T>(string url)
        {
            var data = SendData(url, HttpMethod.Get);
            return JsonConvert.DeserializeObject<DataResult<T>>(data).Data;
        }

        /// <summary>
        /// HttpMethod.Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public T Fetch<T>(string url, object query)
        {
            var content = JsonConvert.SerializeObject(query);
            var data = SendData(url, HttpMethod.Post, content);
            return JsonConvert.DeserializeObject<DataResult<T>>(data).Data;
        }

        /// <summary>
        /// HttpMethod.Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList<T> FetchList<T>(string url, object query)
        {
            var content = JsonConvert.SerializeObject(query);
            var data = SendData(url, HttpMethod.Post, content);
            return JsonConvert.DeserializeObject<ListResult<T>>(data).Data;
        }
        /// <summary>
        /// HttpMethod.Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public IList<T> FetchList<T>(string url)
        {
            string data = SendData(url, HttpMethod.Get);
            return JsonConvert.DeserializeObject<ListResult<T>>(data).Data;
        }

        /// <summary>
        /// HttpMethod.Put
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        public void Update(string url, object obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            var data = SendData(url, HttpMethod.Put, content);
        }

        /// <summary>
        /// HttpMethod.Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object Add(string url, object obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            var data = SendData(url, HttpMethod.Post, content);
            return JsonConvert.DeserializeObject<DataResult<object>>(data).Data;
        }

        /// <summary>
        /// HttpMethod.Delete
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Delete(string url)
        {
            var data = SendData(url, HttpMethod.Delete);
            var result = JsonConvert.DeserializeObject<ActionResult>(data);
            return result.Message;
        }

        public void DownLoad(string address, string savepath)
        {
            WebClient dc = new WebClient();
            dc.BaseAddress = BaseAddress;
            dc.DownloadFile(address, savepath);
        }

        public byte[] DownLoadData(string address)
        {
            WebClient dc = new WebClient();
            dc.BaseAddress = BaseAddress;
            return dc.DownloadData(address);
        }
    }
}
