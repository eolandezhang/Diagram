using QPP.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf
{
    public class ModifiedMessage<T> : GenericMessage<T>
    {
        public bool IsCreated { get; set; }
        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="content">The message content.</param>
        public ModifiedMessage(object sender, T content, bool isCreated)
            : base(sender, content)
        {
            IsCreated = isCreated;
        }

        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="content">The message content.</param>
        public ModifiedMessage(object sender, object target, T content, bool isCreated)
            : base(sender, target, content)
        {
            IsCreated = isCreated;
        }
    }
}
