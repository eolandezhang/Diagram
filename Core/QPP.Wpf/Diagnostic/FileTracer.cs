using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Diagnostic
{
    public class FileTracer
    {
        public static string FileName = "app.log";
        public static string BackFileName = "app.bak.log";
        public static long MaxFileSize = ConfigurationManager.AppSettings["LogFileMaxSize"].ConvertTo<long>(10240000);

        static object asyncLock = new object();

        public static void Write(string message)
        {
            Action action = () =>
            {
                lock (asyncLock)
                {
                    if (File.Exists(FileName))
                    {
                        FileInfo f = new FileInfo(FileName);
                        if (f.Length > MaxFileSize)
                        {
                            if (File.Exists(BackFileName))
                                File.Delete(BackFileName);
                            File.Move(FileName, BackFileName);
                        }
                    }
                    using (var sw = new StreamWriter(FileName, true))
                    {
                        sw.Write(message);
                        sw.Close();
                    }
                }
            };
            action.BeginInvoke(new AsyncCallback(e => { }), null);
        }

    }
}
