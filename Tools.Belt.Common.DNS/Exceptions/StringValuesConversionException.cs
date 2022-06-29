// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Tools.Belt.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class StringValuesConversionException : Exception
    {
        public StringValuesConversionException()
        {
        }

        public StringValuesConversionException(string message) : base(message)
        {
        }

        public StringValuesConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StringValuesConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}