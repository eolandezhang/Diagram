using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using QPP.ComponentModel;
using QPP.Localization;

namespace QPP.Validation
{
    public class DataValidator : IDataValidator
    {
        internal Dictionary<string, string> errors = new Dictionary<string, string>();
        Dictionary<string, ValidationAttribute[]> validators;
        ILocalization localization;

        protected DataValidator()
        {
        }

        public static IDataValidator Create(ILocalization local = null)
        {
            return new DataValidator().SetLocalization(local);
        }

        public DataValidator SetLocalization(ILocalization local)
        {
            localization = local;
            return this;
        }

        Dictionary<string, ValidationAttribute[]> GetValidator(Type type)
        {
            if (validators == null)
                validators = Validator.GetValidator(type);
            return validators;
        }

        string GetErrorMessage(Type type, string propertyName, ValidationAttribute validator)
        {
            if (localization != null)
                propertyName = localization.GetText(type.Name + "." + propertyName);
            return validator.FormatErrorMessage(propertyName);
        }

        public void Validate(object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            var type = obj.GetType();
            var validators = GetValidator(type);
            if (validators.ContainsKey(propertyName))
            {
                object value = null;
                var obs = obj as ObservableObject;
                if (obs != null)
                    value = obs.Get<object>(propertyName);
                else
                    value = obj.GetType().GetProperty(propertyName).GetValue(obj, null);
                foreach (var v in validators[propertyName])
                {
                    if (!v.IsValid(value))
                    {
                        errors[propertyName] = GetErrorMessage(type, propertyName, v);
                        return;
                    }
                }
            }
            errors.Remove(propertyName);
        }

        public void Validate(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            var type = obj.GetType();
            var validators = GetValidator(type);
            var obs = obj as ObservableObject;
            foreach (var d in validators)
            {
                object value = null;
                if (obs != null)
                    value = obs.Get<object>(d.Key);
                else
                    value = obj.GetType().GetProperty(d.Key).GetValue(obj, null);
                foreach (var v in d.Value)
                {
                    if (!v.IsValid(value))
                    {
                        errors[d.Key] = GetErrorMessage(type, d.Key, v);
                        break;
                    }
                }
            }
        }

        public string Error
        {
            get { return string.Join(Environment.NewLine, errors.Values); }
        }

        public string GetError(string columnName)
        {
            if (errors.ContainsKey(columnName))
                return errors[columnName];
            return null;
        }
    }
}
