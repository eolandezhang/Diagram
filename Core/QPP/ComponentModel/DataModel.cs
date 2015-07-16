using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using System.ComponentModel;
using QPP.Validation;

namespace QPP.ComponentModel
{
    /// <summary>
    /// 有狀態，並且有數據驗證功能的數據模型
    /// </summary>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public class DataModel : StatefulObject, IDataModel
    {
        /// <summary>
        /// 數據版本，用於處理并發
        /// </summary>
        protected virtual int VERSION { get; set; }

        [NonSerialized]
        IDataValidator validator;

        /// <summary>
        /// 設置驗證器
        /// </summary>
        /// <param name="dataValidator"></param>
        public virtual void SetValidator(IDataValidator dataValidator)
        {
            validator = dataValidator;
        }

        protected override void OnValueChanged(string propertyName, object newValue, object oldValue)
        {
            if (DataState == DataState.Initializing) return;
            if (validator != null)
                validator.Validate(this, propertyName);//屬性值變更后，立刻進行驗證
            base.OnValueChanged(propertyName, newValue, oldValue);
        }

        public virtual void Validate()
        {
            if (validator != null)
                validator.Validate(this);
            RaisePropertyChanged(null);
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        string IDataErrorInfo.Error
        {
            get
            {
                if (validator != null)
                    return validator.Error;
                return null;
            }
        }
        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The name of the property whose error message to get.</param>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (validator != null)
                    return validator.GetError(columnName);
                return null;
            }
        }

        public override int GetHashCode()
        {
            var key = EntityId.GetId(this);
            return key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;
            bool hasKey = false;
            foreach (var p in obj.GetType().GetProperties())
            {
                if (Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.KeyAttribute)))
                {
                    hasKey = true;
                    if (!object.Equals(p.GetValue(this, null), p.GetValue(obj, null)))
                        return false;
                }
            }
            if (hasKey)
                return true;
            return base.Equals(obj);
        }
    }
}
