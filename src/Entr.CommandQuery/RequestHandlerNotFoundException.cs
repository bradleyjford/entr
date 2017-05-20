using System;

namespace Entr.CommandQuery
{
    public class RequestHandlerNotFoundException : Exception
    {
        const string MessageFormat = @"Handler for request of type ""{0}"" not found.";

        public RequestHandlerNotFoundException(Type commandType, Exception innerException)
            : base(String.Format(MessageFormat, commandType.FullName), innerException)
        {
        }
    }
}
