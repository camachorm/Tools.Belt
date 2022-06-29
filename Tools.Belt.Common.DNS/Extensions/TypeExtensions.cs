using System;

namespace Tools.Belt.Common.Extensions
{
    public static class TypeExtensions
    {
        public static string ReadEmbeddedResource(this Type source, string resourceName)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Assembly.ReadEmbeddedResource(resourceName);
        }
    }
}