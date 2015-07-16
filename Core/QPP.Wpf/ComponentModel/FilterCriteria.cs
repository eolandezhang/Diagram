using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace QPP.Wpf.ComponentModel
{
    public class FilterCriteria : INotifyPropertyChanged,
        INotifyPropertyChanging
    {
        private List<FieldExpression> _expressionCollection = new List<FieldExpression>();

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        public virtual List<FieldExpression> Expressions { get { return _expressionCollection; } }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Dictionary<string, object> m_Values = new Dictionary<string, object>();

        public ICriteriaTemplate Template { get; set; }

        public FilterCriteria SetTemplate(ICriteriaTemplate template)
        {
            Template = template;
            return this;
        }

        public void Reset()
        {
            foreach (var key in m_Values.Keys.ToList())
            {
                var scope = m_Values[key] as Scope;
                if (scope != null)
                {
                    scope.From = null;
                    scope.To = null;
                }
                else
                    m_Values.Remove(key);
            }
            OnPropertyChanged("Item[]");
        }

        TemplateHandler GetTemplate(string key)
        {
            if (Template == null) return null;
            return Template.GetTemplate(key);
        }

        public string ToCriteriaString()
        {
            string result = "";
            foreach (var v in m_Values)
            {
                if (!HasValue(v.Value))
                    continue;

                var criteria = "";

                TemplateHandler template = GetTemplate(v.Key);
                if (template != null)
                    criteria = template(v.Value);
                else
                {
                    var expression = Expressions.FirstOrDefault(p => p.DataField == v.Key);
                    criteria = ToCriteriaString(v.Key, v.Value);
                    if (expression != null)
                    {
                        criteria = expression.Expression.FormatArgs(criteria);
                    }
                }

                if (criteria.IsNotEmpty())
                {
                    if (result.IsNotEmpty())
                        result += " And ";
                    result += criteria;
                }
            }
            return result;
        }

        public bool HasCriteriaValue
        {
            get
            {
                foreach (var v in m_Values)
                    if (HasValue(v.Value))
                        return true;
                return false;
            }
        }

        static bool HasValue(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Scope)
            {
                var scope = obj as Scope;
                return HasValue(scope.From) || HasValue(scope.To);
            }
            if (obj is string)
                return ((string)obj).IsNotEmpty();
            return true;
        }

        static Regex inReg = new Regex("[;；,，]");
        static Regex fuzzyReg = new Regex("[%_]");

        public static string ToCriteriaString(string property, object value)
        {
            if (value == null || value == DBNull.Value)
                return "";
            if (value is Scope)
            {
                var result = "";
                var s = value as Scope;
                string scope = "";
                if (HasValue(s.From))
                {
                    scope += " [" + property + "] >= " + FormatValue(s.From);
                }
                if (HasValue(s.To))
                {
                    if (scope.IsNotEmpty())
                        scope += " And ";
                    scope += "[" + property + "] <= " + FormatValue(s.To, true);
                }
                result += scope;
                return result;
            }
            else if (value is string)
            {
                var result = "";
                var v = value.ToSafeString();
                if (v.IsNotEmpty())
                {
                    if (inReg.IsMatch(v))
                    {
                        var m = inReg.Replace(v, ";");
                        var x = m.Split(';');
                        var param = "";
                        for (int i = 0; i < x.Length; i++)
                            param += FormatValue(x[i]) + ",";
                        result += "[" + property + "] In (" + param.TrimEnd(',') + ")";
                    }
                    else if (fuzzyReg.IsMatch(v))
                        result += "[" + property + "] Like " + FormatValue(value);
                    else
                        result += "[" + property + "] = " + FormatValue(value);
                }
                return result;
            }
            return "[" + property + "] = " + FormatValue(value);
        }

        static string FormatValue(object value, bool to = false)
        {
            if (value is string)
                return "'" + value.ToSafeString().Replace("'", "''") + "'";
            if (value is DateTime)
            {
                if (to)
                    return "#" + ((DateTime)value).ToString("yyyy-MM-dd 23:59:59") + "#";
                return "#" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "#";
            }
            return value.ToSafeString();
        }

        object GetValue(string value)
        {
            if (value.StartsWith("'") && value.EndsWith("'"))
                return value.Substring(1, value.Length - 2).Replace("''", "'");
            if (value.StartsWith("#") && value.EndsWith("#"))
                return DateTime.Parse(value.Trim('#'));
            if (value.Contains('.'))
                return double.Parse(value);
            return int.Parse(value);
        }

        public string ToQueryString()
        {
            string result = "";
            foreach (var v in m_Values)
            {
                if (!HasValue(v.Value))
                    continue;
                if (result.IsNotEmpty())
                    result += "&";
                if (v.Value is Scope)
                {
                    var s = v.Value as Scope;
                    result += v.Key + "@from=" + FormatValue(s.From);
                    result += "&" + v.Key + "@to=" + FormatValue(s.To);
                }
                else
                {
                    result += v.Key + "=" + FormatValue(v.Value);
                }
            }
            return result;
        }

        public void LoadQuery(string queryString)
        {
            try
            {
                Reset();
                var pair = queryString.Split('&');
                foreach (var p in pair)
                {
                    if (p.IsNullOrWhiteSpace())
                        continue;
                    var v = p.Split('=');
                    m_Values[v[0]] = GetValue(v[1]);
                }
                OnPropertyChanged("Item[]");
            }
            catch (Exception e)
            {
                throw new FormatException(queryString, e);
            }
        }

        [IndexerName("Item")]
        public object this[string propertyName]
        {
            get
            {
                if (m_Values.ContainsKey(propertyName))
                    return m_Values[propertyName];
                else
                {
                    return null;
                }
            }
            set
            {
                if (!object.Equals(this[propertyName], value))
                {
                    OnPropertyChanging("Item[]");
                    OnPropertyChanging(propertyName);
                    m_Values[propertyName] = value;
                    OnPropertyChanged("Item[]");
                    OnPropertyChanged(propertyName);
                }
            }
        }

        public void CopyTo(FilterCriteria criteria)
        {
            criteria.m_Values.Clear();
            foreach (var item in m_Values)
                criteria.m_Values[item.Key] = item.Value;
            criteria.OnPropertyChanged("Item[]");
        }
    }
}
