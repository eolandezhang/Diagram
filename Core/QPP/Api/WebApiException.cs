using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Api
{
    [Serializable]
    public class WebApiException : AppException
    {
        public string RespondText { get; set; }

        public WebApiException() { }

        public WebApiException(string message)
            : base(message)
        {
        }

        public WebApiException(string message, string respondText)
            : base(message)
        {
            RespondText = respondText;
        }

        public WebApiException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected WebApiException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            RespondText = info.GetString("RespondText");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RespondText", RespondText);
        }
    }
}
