using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public class FilterNode : DataModel ,IFilterNode
    {
        public string Name
        {
            get { return Get<string>("Name"); }
            set { Set("Name", value); }
        }
        public string FieldName
        {
            get { return Get<string>("FieldName"); }
            set { Set("FieldName", value); }
        }
        public ActionType Action
        {
            get { return Get<ActionType>("Action"); }
            set { Set("Action", value); }
        }
        public object Value
        {
            get { return Get<object>("Value"); }
            set { Set("Value", value); }
        }
        public Scope Scope
        {
            get { return Get<Scope>("Scope"); }
            set { Set("Scope", value); }
        }        
        public object From
        {
            get { return Get<object>("From"); }
            set { Set("From", value); }
        }
        public object To
        {
            get { return Get<object>("To"); }
            set { Set("To", value); }
        }
        public ObservableCollection<IFilterNode> Children
        {
            get { return Get<ObservableCollection<IFilterNode>>("Children"); }
            set { Set("Children", value); }
        }
        public IFilterNode Parent
        {
            get { return Get<IFilterNode>("Parent"); }
            set { Set("Parent", value); }
        }
        public FilterNode()
        {
            Children = new ObservableCollection<IFilterNode>();
            Scope = new Scope();
        }
        public int Level
        {
            get 
            {
                if (Parent == null)
                    return 1;
                else
                    return Parent.Level + 1;
            }
            set { Set("Level", value); }
        }        
        public bool IsGroup
        {
            get { return Get<bool>("IsGroup"); }
            set { Set("IsGroup", value); }
        }
        public int HorizontalOffset
        {
            get
            {
                if (Parent == null)
                    return 20;
                else
                    return Parent.HorizontalOffset + 20;
            }
        }
        public string ChildrenRelation
        {
            get { return Get<string>("ChildrenRelation"); }
            set { Set("ChildrenRelation", value); }
        }
        public TypeCode Type
        {
            get { return Get<TypeCode>("Type"); }
            set { Set("Type", value); }
        }
        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "Type")
            {
                IsBoolType = Type == TypeCode.Boolean;
                IsString = Type == TypeCode.String;
                IsDateTimeOrNumericType = Type == TypeCode.DateTime 
                    || Type == TypeCode.Int16 
                    || Type == TypeCode.Int32 
                    || Type == TypeCode.Int64 
                    || Type == TypeCode.Single 
                    || Type == TypeCode.Double 
                    || Type == TypeCode.Decimal;
            }
            base.OnPropertyChanged(propertyName);
        }
        public bool IsBoolType
        {
            get { return Get<bool>("IsBoolType"); }
            set { Set("IsBoolType", value); }
        }
        public bool IsDateTimeOrNumericType
        {
            get { return Get<bool>("IsDateTimeOrNumericType"); }
            set { Set("IsDateTimeOrNumericType", value); }
        }
        public bool IsString
        {
            get { return Get<bool>("IsString"); }
            set { Set("IsString", value); }
        }
        public FilterDataTemplate FilterDataTemplate { get; set; }        
    }
}
