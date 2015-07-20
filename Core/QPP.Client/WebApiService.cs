using Newtonsoft.Json;
using QPP.Api;
using QPP.Security;
using QPP.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace QPP.Client
{
    public class WebApiService : IWebApiService
    {
        public string BaseAddress { get; set; }

        public int Timeout { get; set; }

        public string Database { get; set; }

        ITicketContainer m_Ticket;

        public WebApiService SetTicketContainer(ITicketContainer context)
        {
            m_Ticket = context;
            return this;
        }

        public WebApiService(string baseAddress, int timeout = 600)
        {
            BaseAddress = baseAddress;
            Timeout = timeout;
        }

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
            {
                //簡體轉換繁體
                request.Content = new StringContent(data.ToTraditional(), Encoding.UTF8, "application/json");
            }

            if (Database.IsNotEmpty())
                request.Headers.Add("X-Connection", Database);

            if (m_Ticket != null)
            {
                var ticket = m_Ticket.GetTicket();
                if (ticket.IsNotEmpty())
                {
                    if (m_Ticket.Mode == Security.AuthenticationMode.Basic)
                        request.Headers.Add("Authorization", "Basic " + ticket);
                    else
                        request.Headers.Add("Authorization", "Ticket " + ticket);
                }
            }

            var response = m_HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
            if (response.IsSuccessStatusCode)
            {
                if (m_Ticket != null && m_Ticket.Mode == Security.AuthenticationMode.Ticket)
                {
                    var cookies = m_Handler.CookieContainer.GetCookies(m_HttpClient.BaseAddress);
                    var authCookie = cookies.OfType<Cookie>().FirstOrDefault(p => p.Name == ".ASPXAUTH");
                    if (authCookie != null)
                    {
                        System.Diagnostics.Debug.WriteLine("authCookie from {0}{1}:{2}".FormatArgs(m_HttpClient.BaseAddress, url, authCookie.Value));
                        m_Ticket.SetTicket(authCookie.Value);
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
                //RuntimeContext.Current.Diagnostic.Trace.WriteLine("BaseAddress:{0},Url:{1},Ticket:{2}".FormatArgs(m_HttpClient.BaseAddress, url, ticket));
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new System.Security.Authentication.AuthenticationException(response.ReasonPhrase);
                }
                string text = null;
                try
                {
                    text = response.Content.ReadAsStringAsync().Result;
                }
                catch { }
                var msg = string.Format("WebApi Request Error:{0} ({1})\r\nBaseAddress:{2}\r\nUrl:{3}\r\nMethod:{4}",
                    (int)response.StatusCode, response.ReasonPhrase, m_HttpClient.BaseAddress.OriginalString, url, method);
                throw new WebApiException(msg, text);
            }
        }

        public T Get<T>(string url)
        {
            var result = SendData(url, HttpMethod.Get);
            return JsonConvert.DeserializeObject<T>(result);
        }

        public T Post<T>(string url, object data)
        {
            var content = JsonConvert.SerializeObject(data);
            var result = SendData(url, HttpMethod.Post, content);
            return JsonConvert.DeserializeObject<T>(result);
        }

        public void Put(string url, object data)
        {
            var content = JsonConvert.SerializeObject(data);
            SendData(url, HttpMethod.Put, content);
        }

        public void Delete(string url)
        {
            SendData(url, HttpMethod.Delete);
        }
    }
}
