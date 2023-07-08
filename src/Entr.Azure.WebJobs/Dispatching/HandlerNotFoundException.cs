using System;
using System.Runtime.Serialization;

namespace Entr.Azure.WebJobs.Dispatching
{
    [Serializable]
    public class HandlerNotFoundException : Exception
    {
        private const string MessageFormat = @"Handler for request of type ""{0}"" not found.";

        internal HandlerNotFoundException(Type commandType)
            : base(String.Format(MessageFormat, commandType.FullName))
        {
        }

        protected HandlerNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
