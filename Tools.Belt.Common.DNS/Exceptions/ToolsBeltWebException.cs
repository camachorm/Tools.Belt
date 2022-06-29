using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace Tools.Belt.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ToolsBeltWebException : ToolsBeltException
    {
        public ToolsBeltWebException()
        {
        }

        public ToolsBeltWebException(string message) : base(message)
        {
        }

        public ToolsBeltWebException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ToolsBeltWebException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
            Justification = "Legacy usage scenarios required this to be settable.")]
        public WebHeaderCollection ResponseHeaders { get; set; }
    }
}