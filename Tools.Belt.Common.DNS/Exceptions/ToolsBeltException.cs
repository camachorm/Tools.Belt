// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Tools.Belt.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ToolsBeltException : Exception
    {
        public ToolsBeltException()
        {
        }

        public ToolsBeltException(string message) : base(message)
        {
        }

        public ToolsBeltException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ToolsBeltException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ConcurrentDictionary<string, object> AssociatedObjects { get; } =
            new ConcurrentDictionary<string, object>();
    }
}