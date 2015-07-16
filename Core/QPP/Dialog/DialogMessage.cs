using QPP.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Dialog
{
    public class DialogMessage : GenericMessage<string>
    {
        public DialogMessage()
        {
        }

        public DialogMessage(
           object sender, string content)
            : base(sender, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XMessage class.
        /// </summary>
        /// <param name="content">The text displayed by the message box.</param>
        /// <param name="callback">A callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.</param>
        public DialogMessage(
            string content,
            Action<DialogResult> callback)
            : base(content)
        {
            Callback = callback;
        }

        /// <summary>
        /// Initializes a new instance of the DialogMessage class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="content">The text displayed by the message box.</param>
        /// <param name="callback">A callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.</param>
        public DialogMessage(
            object sender,
            string content,
            Action<DialogResult> callback)
            : base(sender, content)
        {
            Callback = callback;
        }

        /// <summary>
        /// Initializes a new instance of the DialogMessage class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="content">The text displayed by the message box.</param>
        /// <param name="callback">A callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.</param>
        public DialogMessage(
            object sender,
            object target,
            string content,
            Action<DialogResult> callback)
            : base(sender, target, content)
        {
            Callback = callback;
        }

        /// <summary>
        /// Gets or sets the buttons displayed by the message box.
        /// </summary>
        public DialogButton Button
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.
        /// </summary>
        public Action<DialogResult> Callback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the caption for the message box.
        /// </summary>
        public string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets which result is the default in the message box.
        /// </summary>
        public DialogResult DefaultResult
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the icon for the message box.
        /// </summary>
        public DialogImage Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Utility method, checks if the <see cref="Callback" /> property is
        /// null, and if it is not null, executes it.
        /// </summary>
        /// <param name="result">The result that must be passed
        /// to the dialog message caller.</param>
        public void ProcessCallback(DialogResult result)
        {
            if (Callback != null)
            {
                Callback(result);
            }
        }
    }
}
