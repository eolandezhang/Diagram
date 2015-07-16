using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.FilterControl
{
    public enum ActionType
    {
        IsNull = 0,
        NotNull = 1,
        Equal = 2,
        NotEqual = 3,
        Lesser = 4,
        LesserOrEqual = 5,
        Greater = 6,
        GreaterOrEqual = 7,
        Between = 8,
        NotBetween = 9,
        BeginWith = 10,
        EndWith = 11,
        Contain = 12,
        NotContain = 13,
        IsTrue = 14,
        IsFalse = 15
    }
}
