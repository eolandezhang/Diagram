using ICCEmbedded.SharpZipLib.Zip;
using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Threading;

namespace QPP.Wpf.Modularity
{
    public class ModuleTypeLoader : IModuleTypeLoader
    {
        const string m_UpdateFileFolder = "TempUpdateFile";
        static List<string> m_Assemblies = new List<string>();
        static string m_UpdateAddress = ConfigurationManager.AppSettings["updateAddress"];

        public event EventHandler<LoadComponentEventArgs> PreLoad;

        public event EventHandler LoadCompleted;

        public event EventHandler<LoadComponentEventArgs> BeforeDownload;

        public event EventHandler DownloadCompleted;

        public event EventHandler<ModuleDownloadProgressChangedEventArgs> DownloadProgressChanged;

        static ModuleTypeLoader()
        {
            foreach (var f in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories))
                m_Assemblies.Add(AssemblyName.GetAssemblyName(f).Name);
        }

        public object LoadComponent(Uri uri)
        {
            try
            {
                OnPreLoad(new LoadComponentEventArgs(uri));
                LoadAssembly(uri);
                return System.Windows.Application.LoadComponent(uri);
            }
            finally
            {
                OnLoadCompleted(EventArgs.Empty);
            }
        }

        public void LoadAssembly(Uri uri)
        {
            var componentIndex = uri.OriginalString.IndexOf(";component", StringComparison.OrdinalIgnoreCase);
            if (componentIndex > -1)
            {
                var startIndex = uri.OriginalString.LastIndexOf('/', componentIndex) + 1;
                var assembly = uri.OriginalString.Substring(startIndex, componentIndex - startIndex);
                if (m_Assemblies.Any(p => p.CIEquals(assembly)))
                    return;
                try
                {
                    OnBeforeDownload(new LoadComponentEventArgs(uri));
                    var localModuleInfo = GetLocalModuleInfo();
                    var serverModuleInfo = GetServerModuleInfo();

                    var moduleInfo = serverModuleInfo.FirstOrDefault(p => p.Name.CIEquals(assembly));
                    if (moduleInfo == null)
                        throw new FileNotFoundException("服務器上找不到程序集{0}".FormatArgs(assembly));
                    var modules = GetReferredModules(moduleInfo);
                    //TODO: 檢測版本更新
                    var needDownload = modules.Where(p => !localModuleInfo.Any(q => p.Name.CIEquals(q.Name))).Distinct(Manifest.ModuleInfo.Comparer).ToList();
                    OnDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(10, null));
                    DownloadAndExtractFile(needDownload);
                    UpdateManifest(localModuleInfo, needDownload);
                    OnDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(100, null));
                }
                finally
                {
                    DeleteDirectory(m_UpdateFileFolder);
                    OnDownloadCompleted(EventArgs.Empty);
                }
            }
        }

        IList<Manifest.ModuleInfo> GetLocalModuleInfo()
        {
            var manifest = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Manifest.FileName));
            return Manifest.Load(manifest);
        }

        IList<Manifest.ModuleInfo> GetServerModuleInfo()
        {
            var data = DownloadFile(Manifest.FileName, 0, 5);
            using (var stream = new MemoryStream(data))
                return Manifest.Load(stream);//服務器上定義的模塊信息
        }

        /// <summary>
        /// 复制目录中的所以文件和子目录
        /// </summary>
        IList<string> CopyDirectory(string sourceDirName, string destDirName)
        {
            List<string> result = new List<string>();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
            }
            string[] files = Directory.GetFiles(sourceDirName);
            foreach (string file in files)
            {
                var fileName = Path.GetFileName(file);
                var dest = Path.Combine(destDirName, fileName);
                //if (m_Assemblies.Contains(Path.GetFileNameWithoutExtension(file)))
                //{
                //    //已經安裝,更新需要關閉程序
                //    throw new AppUpdateFailedException();
                //}
                File.Copy(file, dest, true);
                File.SetAttributes(dest, FileAttributes.Normal);
                result.Add(dest);
                try
                {
                    lock (m_Assemblies)
                        m_Assemblies.Add(AssemblyName.GetAssemblyName(dest).Name);
                }
                catch (BadImageFormatException) { }
            }
            string[] dirs = Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
            {
                result.AddRange(CopyDirectory(dir, Path.Combine(destDirName, Path.GetFileName(dir))));
            }
            return result;
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        void DeleteDirectory(string path)
        {
            try
            {
                System.IO.Directory.Delete(path, true);
            }
            catch { }
        }

        IList<Manifest.ModuleInfo> UpdateManifest(IList<Manifest.ModuleInfo> local, IList<Manifest.ModuleInfo> newer)
        {
            var modules = new List<Manifest.ModuleInfo>();
            foreach (var n in newer)
            {
                n.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                modules.Add(n);
            }
            foreach (var n in local.Where(p => !modules.Any(q => p.Name.CIEquals(q.Name))).ToList())
            {
                modules.Add(n);
            }
            Manifest.Save(modules, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Manifest.FileName));
            return modules;
        }

        [System.Security.Permissions.HostProtection(ExternalThreading = true)]
        byte[] DownloadFile(string file, int progress, int maxProgress)
        {
            var client = new WebClient();
            client.BaseAddress = m_UpdateAddress;
            DownloadDataCompletedEventArgs completed = null;
            client.DownloadProgressChanged += (sender, e) =>
            {
                var d = (e.BytesReceived / (double)e.TotalBytesToReceive) * maxProgress;
                OnDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(progress + (int)d));
            };
            client.DownloadDataCompleted += (sender, e) =>
            {
                completed = e;
            };
            client.DownloadDataAsync(new Uri(file, UriKind.Relative));
            while (true)
            {
                if (completed != null)
                {
                    if (completed.Error != null)
                        throw completed.Error;
                    return completed.Result;
                }
                System.Windows.Application.Current.Dispatcher.DoEvent();
                System.Threading.Thread.Sleep(50);
            }
        }

        void DownloadAndExtractFile(IList<Manifest.ModuleInfo> newer)
        {
            double x = 80 / Math.Max(1, newer.Count());
            int progress = 10;
            foreach (var module in newer)
            {
                OnDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(progress));
                List<string> extractFiles = new List<string>();
                double y = x / Math.Max(1, module.Files.Count());
                foreach (var file in module.Files)
                {
                    var data = DownloadFile(file, progress, (int)y);
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, m_UpdateFileFolder, Guid.NewGuid().ToString("N"));
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    var savedFile = Path.Combine(filePath, file);
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(savedFile, FileMode.OpenOrCreate)))
                    {
                        writer.Write(data);
                        writer.Flush();
                        writer.Close();
                    }
                    var extractPath = Path.Combine(filePath, "extract");
                    FastZipEvents events = new FastZipEvents();
                    FastZip zip = new FastZip(events);
                    zip.ExtractZip(savedFile, extractPath, "");
                    extractFiles.AddRange(CopyDirectory(extractPath, AppDomain.CurrentDomain.BaseDirectory));
                    progress = (int)(progress + y);
                    OnDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(progress));
                }
                module.Files.Clear();
                module.Files.AddRange(extractFiles);
            }
        }

        IList<Manifest.ModuleInfo> GetReferredModules(Manifest.ModuleInfo m)
        {
            List<Manifest.ModuleInfo> result = new List<Manifest.ModuleInfo>();
            result.Add(m);
            foreach (var r in m.Reference)
                result.AddRange(GetReferredModules(r));
            return result;
        }

        protected virtual void OnPreLoad(LoadComponentEventArgs e)
        {
            if (PreLoad != null)
                PreLoad(this, e);
        }

        protected virtual void OnLoadCompleted(EventArgs e)
        {
            if (LoadCompleted != null)
                LoadCompleted(this, e);
        }

        protected virtual void OnBeforeDownload(LoadComponentEventArgs e)
        {
            if (BeforeDownload != null)
                BeforeDownload(this, e);
        }

        protected virtual void OnDownloadCompleted(EventArgs e)
        {
            if (DownloadCompleted != null)
                DownloadCompleted(this, e);
        }

        protected virtual void OnDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(this, e);
        }
    }
}
