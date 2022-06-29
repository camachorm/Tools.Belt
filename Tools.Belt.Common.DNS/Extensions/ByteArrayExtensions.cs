using System;
using System.Text;
using Tools.Belt.Common.Environment;

namespace Tools.Belt.Common.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string Decode(this byte[] source)
        {
            return source.Decode(RuntimeConfiguration.DefaultEncoding);
        }

        public static string Decode(this byte[] source, Encoding encoding)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return encoding.GetString(source);
        }

        public static byte[] Encode(this string source, Encoding encoding)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return encoding.GetBytes(source);
        }

        public static byte[] Encode(this string source)
        {
            return source.Encode(RuntimeConfiguration.DefaultEncoding);
        }
    }
}