
using QPP.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Balloon
{
    public class BalloonMessage : GenericMessage<object>
    {
        public string Uri
        {
            get;
            set;
        }

        public int? Timeout
        {
            get;
            set;
        }
    }
}
