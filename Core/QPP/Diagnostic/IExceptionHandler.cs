using QPP.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Diagnostic
{
    public interface IExceptionHandler
    {
        /// <summary>
        /// 異常處理
        /// </summary>
        /// <param name="exc"></param>
        string Handle(Exception exc);
        /// <summary>
        /// 驗證信息
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        string GetErrorInfo(ValidationException exc);
    }
}
