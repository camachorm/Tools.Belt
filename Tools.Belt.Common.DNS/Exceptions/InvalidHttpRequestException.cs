// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Tools.Belt.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidHttpRequestException : HttpRequestException
    {
        public InvalidHttpRequestException()
        {
        }

        public InvalidHttpRequestException(string message) : base(message)
        {
        }

        public InvalidHttpRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}