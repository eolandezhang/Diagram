using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using QPP.ServiceLocation;
using QPP.Localization;

namespace QPP.Wpf.Localization
{
    public class L10N : ILocalization
    {
        List<IResourceModel> resources = new List<IResourceModel>();
        string cultureCode;

        private L10N() { }

        static L10N()
        {
            Default = new L10N();
        }

        public void Load(IList<IResourceModel> res)
        {
            resources.Clear();
            resources.AddRange(res);
            Update();
        }

        public IList<IResourceModel> Resources
        {
            get { return resources; }
        }

        public string this[string key]
        {
            get { return GetText(key); }
        }

        public static L10N Default
        {
            get;
            private set;
        }

        public string CultureCode
        {
            get { return cultureCode; }
            set
            {
                if (!cultureCode.CIEquals(value))
                {
                    cultureCode = value;
                    Update();
                }
            }
        }

        public string SplitText(string str)
        {
            string text = str;
            int index = text.LastIndexOf('.');
            if (index > -1 && index + 1 < text.Length)
                text = text.Substring(index + 1);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (sb.Length > 0 && char.IsUpper(text[i]) && char.IsLower(text[i - 1]))
                    sb.Append(' ');
                sb.Append(text[i]);
            }
            return sb.ToString();
        }

        public string GetText(string key)
        {
            if (QPP.Wpf.UI.Util.IsDesignMode)
                return key;

            if (key.IsNullOrEmpty()) return null;
            var r = Resources.FirstOrDefault(p => CultureCode.CIEquals(p.CultureCode)
                && key.CIEquals(p.Name));
            if (r != null)
                return r.Value;

            return SplitText(key);
        }

        string ILocalization.GetText(string key)
        {
            return GetText(key);
        }

        //internal static string GetErrorText(string key, string defaultText)
        //{
        //    if (key.IsNullOrEmpty()) return "";
        //    if (!key.StartsWith("L10N.Error."))
        //        key = "L10N.Error." + key;
        //    if (Application.Current != null)
        //    {
        //        var res = Application.Current.TryFindResource(key);
        //        if (res != null)
        //            return res.ToString();
        //    }
        //    return defaultText;
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        public void Update()
        {
            OnPropertyChanged("Item[]");
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
