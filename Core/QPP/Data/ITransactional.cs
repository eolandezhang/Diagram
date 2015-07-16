using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Data
{
    public interface ITransactional
    {
        ITransaction BeginTransaction();
        ITransaction BeginTransaction(IsolationLevel il);
    }
}
