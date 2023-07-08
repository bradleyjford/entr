using System;

namespace Entr.Azure.WebJobs.Dispatching
{
    public interface IContentTypeConverter
    {
        /// <summary>
        /// Gets the Content Type for the specified message type.
        /// </summary>
        string GetContentType(Type eventType);

        /// <summary>
        /// Gets the Type representing the specified message Content Type.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        Type GetType(string contentType);
    }
}
