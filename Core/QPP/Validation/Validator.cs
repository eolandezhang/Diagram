using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using QPP.ComponentModel;

namespace QPP.Validation
{
    [Serializable]
    public class Validator
    {
        ErrorInfoCollection errors = new ErrorInfoCollection();

        /// <summary>
        /// Checks for a condition and add error if the condition is false.
        /// <para>Return false if has error.</para>
        /// </summary>
        public bool Assert(bool condition, string fieldName, ErrorText error, int rowNum = int.MinValue, string sheetName = "")
        {
            if (!condition)
            {
                AddError(fieldName, error, rowNum, sheetName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value equals default(T).
        /// <para>Return false if has error.</para>
        /// </summary>
        public bool Require<T>(T value, string fieldName, int rowNum = int.MinValue, string sheetName = "")
        {
            if ((value is string && string.IsNullOrEmpty(value as string))
                || (value is DateTime && object.Equals(value, DateTime.MinValue) || object.Equals(value, DateTime.MaxValue))
                || object.Equals(value, default(T)))
            {
                AddError(fieldName, ErrorText.Require, rowNum, sheetName);
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// Add error if value is null or empty.
        ///// <para>Return false if has error.</para>
        ///// </summary>
        //public bool Require(string value, string fieldName, int rowNum = int.MinValue, string sheetName = "")
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        AddError(fieldName, ErrorText.Require, rowNum, sheetName);
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Add error if value is DateTime.MinValue or DateTime.MaxValue.
        ///// <para>Return false if has error.</para>
        ///// </summary>
        //public bool Require(DateTime value, string fieldName, int rowNum = int.MinValue, string sheetName = "")
        //{
        //    if (value == DateTime.MinValue || value == DateTime.MaxValue)
        //    {
        //        AddError(fieldName, ErrorText.Require, rowNum, sheetName);
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// Add error if value's length is greater then lenght.
        /// <para>Return false if has error.</para>
        /// </summary>
        public bool MaxLength(string value, int length, string fieldName, int rowNum = int.MinValue, string sheetName = "")
        {
            if (value.IsNotEmpty() && value.Length > length)
            {
                AddError(fieldName, ErrorText.MaxLength(length, value.Length), rowNum, sheetName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value's length is less then lenght.
        /// </summary>
        public bool MinLength(string value, int length, string fieldName, int rowNum = int.MinValue, string sheetName = "")
        {
            if (value.IsNotEmpty() && value.Length < length)
            {
                AddError(fieldName, ErrorText.MinLength(length, value == null ? 0 : value.Length), rowNum, sheetName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value.CompareTo(minValue) &lt; 0.
        /// </summary>
        public bool MinValue<T>(T value, T minValue, string fieldName, int rowNum = int.MinValue, string sheetName = "") where T : IComparable
        {
            if (value != null && value.CompareTo(minValue) >= 0)
                return true;
            AddError(fieldName, ErrorText.MinValue(minValue, value), rowNum, sheetName);
            return false;
        }

        /// <summary>
        /// Add error if value.CompareTo(maxValue) &gt; 0.
        /// </summary>
        public bool MaxValue<T>(T value, T maxValue, string fieldName, int rowNum = int.MinValue, string sheetName = "") where T : IComparable
        {
            if (value != null && value.CompareTo(maxValue) <= 0)
                return true;
            AddError(fieldName, ErrorText.MaxValue(maxValue, value), rowNum, sheetName);
            return false;
        }

        public void AddError(string fieldName, ErrorText error, int rowNum = int.MinValue, string sheetName = "")
        {
            errors.Add(new ErrorInfo(fieldName, error, rowNum, sheetName));
        }

        public void AddError(IEnumerable<string> fieldName, ErrorText error, int rowNum = int.MinValue, string sheetName = "")
        {
            errors.Add(new ErrorInfo(fieldName, error, rowNum, sheetName));
        }

        /// <summary>
        /// Return true if there's no error.
        /// </summary>
        public bool IsValid
        {
            get { return errors.Count == 0; }
        }

        /// <summary>
        /// Error info.
        /// </summary>
        public ErrorInfoCollection ErrorInfos
        {
            get { return errors; }
        }

        public override string ToString()
        {
            if (IsValid)
                return "Valid";
            else
            {
                StringBuilder text = new StringBuilder();
                foreach (ErrorInfo error in errors)
                {
                    if (error.SheetName.IsNotEmpty())
                        text.Append(string.Format("表【{0}】", error.SheetName)).Append(",");
                    if (error.RowNum != int.MinValue)
                        text.Append(string.Format("第{0}行", error.RowNum)).Append(",");
                    text.Append(error.FieldName);
                    text.Append(":");
                    foreach (ErrorText errorText in error.Errors)
                    {
                        text.Append(errorText.Text.FormatArgs(errorText.Args)).Append(",");
                    }
                    text.Remove(text.Length - 1, 1);
                    text.Append(";");
                }
                return text.ToString();
            }
        }

        internal static Dictionary<string, ValidationAttribute[]> GetValidator(Type type)
        {
            var validators = new Dictionary<string, ValidationAttribute[]>();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ValidationAttribute), true);
                if (customAttributes.Length > 0)
                    validators.Add(propertyInfo.Name, customAttributes as ValidationAttribute[]);
            }
            return validators;
        }

        /// <summary>
        /// Check attributes of System.ComponentModel.DataAnnotations
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        public void Check<TEntity>(TEntity obj)
        {
            var type = typeof(TEntity);
            var validators = GetValidator(type);
            var obs = obj as ObservableObject;
            foreach (var entry in validators)
            {
                object value = null;
                if (obs != null)
                    value = obs.Get<object>(entry.Key);
                else
                    value = obj.GetType().GetProperty(entry.Key).GetValue(obj, null);
                foreach (var v in entry.Value)
                {
                    var property = type.Name + "." + entry.Key;
                    if (v is RequiredAttribute)
                        Require(value, property);
                    else if (v is StringLengthAttribute)
                    {
                        var c = v as StringLengthAttribute;
                        MaxLength(value.ToSafeString(), c.MaximumLength, property);
                        MinLength(value.ToSafeString(), c.MinimumLength, property);
                    }
                    else if (v is RangeAttribute)
                    {
                        var c = v as RangeAttribute;
                        var max = Convert.ChangeType(c.Maximum, c.OperandType) as IComparable;
                        var min = Convert.ChangeType(c.Minimum, c.OperandType) as IComparable;
                        var source = value as IComparable;
                        MaxValue(source, max, property);
                        MinValue(source, min, property);
                    }
                    else if (!v.IsValid(value))
                        AddError(property, new ErrorText(v.ErrorMessage));
                }
            }
        }

        public void IsUnique<T>(Func<bool> action, Expression<System.Func<T, object>> expr)
        {
            if (!action.Invoke())
            {
                var error = new ErrorInfo();
                if (expr.Body.NodeType == ExpressionType.New)
                {
                    foreach (var p in expr.Body.Type.GetProperties())
                        error.FieldName.Add(p.Name);
                }
                else if(expr.Body.NodeType == ExpressionType.MemberAccess)
                {
                    error.FieldName.Add((expr.Body as MemberExpression).Member.Name);
                }
                error.Errors.Add(ErrorText.Exists);
                errors.Add(error);
            }
        }

        public ValidationContext<TEntity, TResult> Check<TEntity, TResult>(TEntity model,
            Expression<System.Func<TEntity, TResult>> expr, int rowNum = int.MinValue)
        {
            return new ValidationContext<TEntity, TResult>(this, model, expr, rowNum);
        }
    }
}
