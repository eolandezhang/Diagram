using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QPP
{
    public class AppException : ApplicationException
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="AppException"/> class.
		/// </summary>
		public AppException() : base("An exception occurred in the Application.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public AppException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public AppException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public AppException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="AppException"/> class 
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
    }
}
