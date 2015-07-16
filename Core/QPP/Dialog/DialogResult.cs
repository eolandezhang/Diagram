using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Dialog
{
    public enum DialogResult
    {

        // 摘要:
        //     訊息方塊沒有傳回結果。
        None = 0,
        //
        // 摘要:
        //     訊息方塊的結果值為 [確定]。
        OK = 1,
        //
        // 摘要:
        //     訊息方塊的結果值為 [取消]。
        Cancel = 2,
        //
        // 摘要:
        //     訊息方塊的結果值為 [是]。
        Yes = 6,
        //
        // 摘要:
        //     訊息方塊的結果值為 [否]。
        No = 7,
    }
}
