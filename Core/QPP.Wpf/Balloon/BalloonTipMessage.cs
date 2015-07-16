using QPP.Messaging;
using QPP.Wpf.UI.Controls.ToolbarIcon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Balloon
{
    public class BalloonTipMessage : GenericMessage<string>
    {
        /// <summary>
        /// Gets or sets the caption for the BalloonTip.
        /// </summary>
        public string Caption
        {
            get;
            set;
        }

        public BalloonIcon Icon
        {
            get;
            set;
        }
    }
}
