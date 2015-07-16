using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using QPP.ComponentModel;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public class FilterCriteriaHelper
    {
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

        static string GetFilterItemStatement(IFilterNode node)
        {
            if (node.Action == ActionType.IsNull)
            {
                return " [" + node.FieldName + "] IS NULL ";
            }
            else if (node.Action == ActionType.NotNull)
            {
                return " [" + node.FieldName + "] IS NOT NULL ";
            }

            else if (node.Action == ActionType.IsTrue)
            {
                return " [" + node.FieldName + "] = true ";
            }
            else if (node.Action == ActionType.IsFalse)
            {
                return " [" + node.FieldName + "] = false ";
            }
            else if (node.Action == ActionType.Between)
            {
                var result = "";
                if (HasValue(node.Scope.From))
                {
                    result += " [" + node.FieldName + "] >= " + FormatValue(node.Scope.From);
                }
                if (HasValue(node.Scope.To))
                {
                    if (result.IsNotEmpty())
                        result += " And ";
                    result += "[" + node.FieldName + "] <= " + FormatValue(node.Scope.To, true);
                }
                if (result == "")
                    return "";
                return " (" + result + ") ";
            }
            else if (node.Action == ActionType.NotBetween)
            {
                var result = "";
                if (HasValue(node.Scope.From))
                {
                    result += " [" + node.FieldName + "] < " + FormatValue(node.Scope.From);
                }
                if (HasValue(node.Scope.To))
                {
                    if (result.IsNotEmpty())
                        result += " Or ";
                    result += "[" + node.FieldName + "] > " + FormatValue(node.Scope.To, true);
                }
                if (result == "")
                    return "";
                return " (" + result + ") ";
            }
            else if (node.Value.ToSafeString() == "")
                return "";

            else if (node.Action == ActionType.Equal)
            {
                if (node.Type == TypeCode.DateTime)
                {
                    var result = "";
                    if (HasValue(node.Value))
                    {
                        result += " [" + node.FieldName + "] >= " + FormatValue(node.Value);
                        result += " And [" + node.FieldName + "] <= " + FormatValue(node.Value, true);
                    }
                    if (result == "")
                        return "";
                    return " (" + result + ") ";
                }
                else
                {
                    return " [" + node.FieldName + "] = " + FormatValue(node.Value) + " ";//node.Value.ToString() + " ";
                }
            }
            else if (node.Action == ActionType.NotEqual)
            {
                if (node.Type == TypeCode.DateTime)
                {
                    var result = "";
                    if (HasValue(node.Value))
                    {
                        result += " [" + node.FieldName + "] <= " + FormatValue(node.Value);
                        result += " Or [" + node.FieldName + "] >= " + FormatValue(node.Value, true);
                    }
                    if (result == "")
                        return "";
                    return " (" + result + ") ";
                }
                else
                {
                    return " [" + node.FieldName + "] != " + FormatValue(node.Value) + " ";//node.Value.ToString() + "' ";
                }
            }
            else if (node.Action == ActionType.EndWith)
            {
                return " [" + node.FieldName + "] Like " + "'%" + node.Value.ToString() + "' ";
            }
            else if (node.Action == ActionType.Contain)
            {
                return " [" + node.FieldName + "] Like " + "'%" + node.Value.ToString() + "%' ";
            }
            else if (node.Action == ActionType.BeginWith)
            {
                return " [" + node.FieldName + "] Like " + "'" + node.Value.ToString() + "%' ";
            }
            else if (node.Action == ActionType.NotContain)
            {
                return " [" + node.FieldName + "] Not Like " + "'%" + node.Value.ToString() + "%' ";
            }

            else if (node.Action == ActionType.Lesser)
            {
                return " [" + node.FieldName + "] < " + FormatValue(node.Value) + " ";//node.Value.ToString();
            }
            else if (node.Action == ActionType.LesserOrEqual)
            {
                return " [" + node.FieldName + "] <= " + FormatValue(node.Value) + " ";//node.Value.ToString();
            }
            else if (node.Action == ActionType.Greater)
            {
                return " [" + node.FieldName + "] > " + FormatValue(node.Value) + " ";//node.Value.ToString();
            }
            else if (node.Action == ActionType.GreaterOrEqual)
            {
                return " [" + node.FieldName + "] >= " + FormatValue(node.Value) + " ";//node.Value.ToString();
            }

            
            return "";
        }

        public static string GetCriteria(IEnumerable<IFilterNode> nodes)
        {
            var nodeList = nodes.Where(p => p.IsGroup && p.Parent == null).ToList();

            if (nodeList.Count == 1 && nodeList[0].Parent == null && nodeList[0].IsGroup)
            {
                var rootNode = nodeList[0];
                return GetGroupCriteria(rootNode).Trim();
            }
            return "";
        }

        public static string GetGroupCriteria(IFilterNode groupNode)
        {
            string result = "";
            if (groupNode == null || !groupNode.IsGroup)
                return "";

            foreach (var child in groupNode.Children)
            {
                string childStatement = "";
                if (!child.IsGroup)
                {
                    childStatement = GetFilterItemStatement(child);
                }
                else
                {
                    childStatement = GetGroupCriteria(child);
                }
                if (childStatement != "")
                {
                    result += groupNode.ChildrenRelation + childStatement;
                }
            }
            if (result != "")
                result = " (" + result.Substring(groupNode.ChildrenRelation.Length) + ") ";
            return result;
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
    }
}
