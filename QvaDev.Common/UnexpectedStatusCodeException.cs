using System;
using System.Net;

namespace QvaDev.Common
{
    public class UnexpectedStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public Uri Uri { get; }

        public UnexpectedStatusCodeException(HttpStatusCode statusCode, Uri uri)
            : base($"Unexpected {statusCode} status code at {uri.OriginalString}")
        {
            StatusCode = statusCode;
            Uri = uri;
        }
    }
}
