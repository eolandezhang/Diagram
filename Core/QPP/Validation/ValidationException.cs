using System;

namespace QPP.Validation
{
    /// <summary>
    /// 验证不通过时引发的异常
    /// </summary>
    [Serializable]
    public class ValidationException : AppException
    {
        ErrorInfoCollection _errors;
        string _innerStackTrace;

        /// <summary>
        /// 错误信息集合
        /// </summary>
        public ErrorInfoCollection ErrorInfos
        {
            get { return _errors; }
            set { _errors = value; }
        }

        /// <summary>
        /// 获取调用堆栈上直接帧的字符串表示形式。
        /// </summary>
        public override string StackTrace
        {
            get
            {
                return String.Format("{0}{1}{2}",
                    _innerStackTrace, Environment.NewLine, base.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">异常信息</param>
        public ValidationException(string message)
            : base(message)
        {
            _errors = new ErrorInfoCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex">内部异常</param>
        public ValidationException(Exception ex)
            : base(ex.Message, ex)
        {
            _errors = new ErrorInfoCollection();
            _innerStackTrace = ex.StackTrace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v">验证器</param>
        public ValidationException(Validator v)
            : base("Invalid")
        {
            _errors = v.ErrorInfos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex">内部异常</param>
        /// <param name="v">验证器</param>
        public ValidationException(Exception ex, Validator v)
            : base(ex.Message, ex)
        {
            _errors = v.ErrorInfos;
            _innerStackTrace = ex.StackTrace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="args">格式化参数</param>
        public ValidationException(string text, params object[] args)
            : base(text.FormatArgs(args))
        {
            _errors = new ErrorInfoCollection();
            _errors.Add(new ErrorInfo("", new ErrorText(text, args)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc">内部异常</param>
        /// <param name="text">文本</param>
        /// <param name="args">格式化参数</param>
        public ValidationException(Exception exc, string text, params object[] args)
            : base(string.Format(text, args), exc)
        {
            _errors = new ErrorInfoCollection();
            _errors.Add(new ErrorInfo("", new ErrorText(text, args)));
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="info"></param>
        public ValidationException(ErrorInfo info)
            : base(info.FieldName + " invalid")
        {
            _errors = new ErrorInfoCollection();
            _errors.Add(info);
        }

        protected ValidationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            _errors = (ErrorInfoCollection)info.GetValue("_errors", typeof(ErrorInfoCollection));
            _innerStackTrace = info.GetString("_innerStackTrace");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_errors", _errors);
            info.AddValue("_innerStackTrace", _innerStackTrace);
        }

    }
}
