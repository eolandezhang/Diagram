using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Api
{
    public interface IApiContext
    {
        /// <summary>
        /// 數據庫
        /// </summary>
        string Database { get; }
        /// <summary>
        /// 公司
        /// </summary>
        string CompanyId { get; }
        /// <summary>
        /// 模拟者
        /// </summary>
        string Simulator { get; }
    }
}
