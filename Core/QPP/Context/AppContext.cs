using QPP.Collections;
using QPP.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Context
{
    internal class AppContext : IAppContext
    {
        public string AppTypeName { get; set; }

        public UserIdentity User { get; set; }

        public NameObjectCollection Content { get; private set; }

        public IAppContext Parent { get; set; }

        public AppContext()
        {
            User = new UserIdentity(null, null);
            Content = new NameObjectCollection();
        }
    }
}
