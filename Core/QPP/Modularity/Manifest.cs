using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QPP.Modularity
{
    /// <summary>
    /// 文件清單
    /// </summary>
    public class Manifest
    {
        public const string FileName = "Manifest.xml";

        public class ModuleInfo
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public List<string> Files { get; private set; }
            public string UpdateTime { get; set; }
            public List<ModuleInfo> Reference { get; private set; }

            public ModuleInfo()
            {
                Files = new List<string>();
                Reference = new List<ModuleInfo>();
            }

            public static ModuleComparer Comparer = new ModuleComparer();

            public class ModuleComparer : IEqualityComparer<ModuleInfo>
            {
                public bool Equals(ModuleInfo x, ModuleInfo y)
                {
                    return x.Name.CIEquals(y.Name);
                }

                public int GetHashCode(ModuleInfo obj)
                {
                    if (obj == null || obj.Name == null)
                        return 0;
                    return obj.Name.ToLower().GetHashCode();
                }
            }
        }

        public static void EnsureManifestFile(string path)
        {
            if (!File.Exists(path))
            {
                using (var sw = new StreamWriter(path))
                {
                    sw.Write(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Assembly>
  <Module>
    <Name>{0}</Name>
  </Module>
</Assembly>".FormatArgs(ConfigurationManager.AppSettings["mainModuleName"]));
                    sw.Close();
                }
            }
        }

        public static IList<ModuleInfo> Load(Stream stream)
        {
            var xml = XDocument.Load(stream);
            stream.Close();
            return Load(xml.Root.Elements());
        }

        public static void Save(IList<ModuleInfo> modules, string file)
        {
            var doc = new XDocument();
            var root = new XElement("Assembly");
            doc.Add(root);
            foreach (var m in modules)
            {
                var e = new XElement("Module");
                e.SetElementValue("Name", m.Name);
                e.SetElementValue("Version", m.Version);
                e.SetElementValue("UpdateTime", m.UpdateTime);
                foreach (var f in m.Files)
                {
                    var child = new XElement("File");
                    child.Value = f;
                    e.Add(child);
                }
                foreach (var r in m.Reference)
                {
                    var child = new XElement("Reference");
                    child.Value = r.Name;
                    e.Add(child);
                }
                doc.Root.Add(e);
            }
            doc.Save(file);
        }

        static IList<ModuleInfo> Load(IEnumerable<XElement> xml)
        {
            List<ModuleInfo> modules = new List<ModuleInfo>();
            foreach (var e in xml)
                modules.Add(LoadModule(xml, e));
            return modules;
        }

        static ModuleInfo LoadModule(IEnumerable<XElement> xml, XElement e)
        {
            ModuleInfo m = new ModuleInfo();
            m.Name = e.GetElementValue("Name");
            m.Version = e.GetElementValue("Version");
            m.Files.AddRange(LoadFiles(e));
            m.UpdateTime = e.GetElementValue("UpdateTime");
            m.Reference.AddRange(LoadReference(xml, e.Elements("Reference")));
            return m;
        }

        static List<string> LoadFiles(XElement e)
        {
            List<string> files = new List<string>();
            foreach (var el in e.Elements("File"))
                files.Add(el.Value);
            return files;
        }

        static IList<ModuleInfo> LoadReference(IEnumerable<XElement> xml, IEnumerable<XElement> reference)
        {
            List<ModuleInfo> modules = new List<ModuleInfo>();
            foreach (var e in xml)
            {
                if (reference.Any(p => p.Value.CIEquals(e.GetElementValue("Name"))))
                    modules.Add(LoadModule(xml, e));
            }
            return modules;
        }
    }
}
