using QPP.Diagnostic;
using QPP.Dialog;
using QPP.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Diagnostic
{
    public class ExceptionHandler : IExceptionHandler
    {
        public string Handle(Exception exc)
        {
            if (!QPP.Wpf.UI.Util.IsDesignMode)
            {
                if (exc is ValidationException)
                {
                    RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage()
                    {
                        Content = exc.Message
                    });
                }
                //else if (exc is System.Security.Authentication.AuthenticationException)
                //{
                //    //Messenger.Default.Send(LoginMessage.Instance);
                //}
                else
                {
                    RuntimeContext.Service.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ")
                        + RuntimeContext.Service.L10N.GetText("發生錯誤:") + exc.ToString() + Environment.NewLine);
                    RuntimeContext.Service.GetObject<IDialog>().Show(new DialogMessage()
                    {
                        Content = exc.ToString()
                    });
                }
            }
            return exc.Message;
        }

        public string GetErrorInfo(Validation.ValidationException exc)
        {
            StringBuilder text = new StringBuilder();
            if (exc.ErrorInfos.Count > 0)
            {
                foreach (ErrorInfo info in exc.ErrorInfos)
                {
                    foreach (ErrorText error in info.Errors)
                    {
                        if (text.Length > 0)
                            text.AppendLine();
                        string errorText = string.Format(RuntimeContext.Service.L10N.GetText(error.Text) + "。", error.Args);
                        foreach (var f in info.FieldName)
                        {
                            text.Append(RuntimeContext.Service.L10N.GetText(f));
                            text.Append(" ");
                        }
                        text.Append(errorText);
                    }
                }
            }
            else
                text.Append(exc.Message);
            return text.ToString();
        }
    }
}
