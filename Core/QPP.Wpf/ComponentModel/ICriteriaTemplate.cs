using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.ComponentModel
{
    public interface ICriteriaTemplate
    {
        TemplateHandler GetTemplate(string propertyName);
    }

    public delegate string TemplateHandler(object value);

    public class CriteriaTemplate : ICriteriaTemplate
    {
        Dictionary<string, TemplateHandler> m_Template = new Dictionary<string, TemplateHandler>();

        public static CriteriaTemplate Create(string key, TemplateHandler value)
        {
            return new CriteriaTemplate().AddTemplate(key, value);
        }

        public CriteriaTemplate AddTemplate(string key, TemplateHandler value)
        {
            m_Template.Add(key, value);
            return this;
        }

        public TemplateHandler GetTemplate(string propertyName)
        {
            if (m_Template.ContainsKey(propertyName))
                return m_Template[propertyName];
            return null;
        }
    }
}
