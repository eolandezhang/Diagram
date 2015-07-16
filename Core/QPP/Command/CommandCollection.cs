using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public class CommandCollection : ObservableCollection<ICommandModel>
    {
        public ICommandModel this[string name]
        {
            get
            {
                var cmd = this.FirstOrDefault(p => p.Name.CIEquals(name));
                return cmd;
            }
        }

        public bool ContainsName(string name)
        {
            return this.Any(p => p.Name.CIEquals(name));
        }
    }
}
