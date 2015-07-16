using System;
using System.Windows;

namespace QPP.Wpf.UI
{
    public class Accent
    {
        public ResourceDictionary Resources;
        public string Name { get; set; }

        public Accent()
        { }

        public Accent(string name, Uri resourceAddress)
        {
            Name = name;
            Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}