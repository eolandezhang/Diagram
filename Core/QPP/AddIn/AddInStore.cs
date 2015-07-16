using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

namespace QPP.AddIn
{
    public class AddInStore
    {
        public static IList<AddInToken> FindAddIns(Type type, string path)
        {
            List<AddInToken> tokens = new List<AddInToken>();
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var assembly = Assembly.LoadFile(file);
                    foreach (var t in assembly.GetTypes())
                    {
                        if (!Attribute.IsDefined(t, typeof(AddInAttribute))) continue;
                        if (t.IsSubclassOf(type) || type.IsAssignableFrom(t))
                            tokens.Add(new AddInToken { Type = t });
                    }
                }
            }
            return tokens;
        }
    }
}
