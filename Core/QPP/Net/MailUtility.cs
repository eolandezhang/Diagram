using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Configuration;

namespace QPP.Net
{
    public class MailUtility
    {
        public static void Send(string subject, string toAddress, string content)
        {
            Send(subject, toAddress, content, (List<string>)null);
        }

        public static void Send(string subject, string toAddress, string content, string attachment)
        {
            Send(subject, toAddress, content, new List<string>(attachment.Split(';', '；')));
        }

        public static void Send(string subject, string toAddress, string content, List<string> attachment)
        {
            SmtpClient client = new SmtpClient();
            var appName = ConfigurationManager.AppSettings["AppName"];
            MailAddress from = new MailAddress("no-reply@qpp.com", appName, System.Text.Encoding.UTF8);
            MailMessage message = new MailMessage();

            message.From = from;
            string[] to = toAddress.ToSafeString().Split(';', '；', ',');
            foreach (string s in to)
            {
                if (!String.IsNullOrEmpty(s) && s.IndexOf('@') > 0)
                {
                    message.To.Add(s);
                }
            }
            RuntimeContext.Service.Logger.Info("Mail Subject:" + subject + " \r\nMail to:" + toAddress);

            message.Body = content;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Priority = MailPriority.High;

            if (attachment != null && attachment.Count > 0)
            {
                foreach (string file in attachment)
                {
                    if (File.Exists(file))
                    {
                        Attachment a = new Attachment(file);
                        message.Attachments.Add(a);
                    }
                }
            }

            client.Send(message);
        }
    }
}
