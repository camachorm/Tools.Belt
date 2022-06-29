using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tools.Belt.Common.Environment;

namespace Tools.Belt.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJsonBase64(this object source)
        {
            string json = JsonConvert.SerializeObject(source);
            byte[] bytes = Encoding.Default.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] Serialize(this object source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            using MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, source);
            return stream.ToArray();
        }

        public static T Deserialize<T>(this byte[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            using MemoryStream stream = new MemoryStream(source);
            IFormatter formatter = new BinaryFormatter();
            return (T) formatter.Deserialize(stream);
        }

        public static T Deserialize<T>(this Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.Seek(0, SeekOrigin.Begin);
            IFormatter formatter = new BinaryFormatter();
            return (T) formatter.Deserialize(source);
        }

        public static byte[] ToJsonByteArray<T>(this T source, Formatting formatting = Formatting.None)
        {
            return source.ToJsonByteArray(RuntimeConfiguration.DefaultEncoding, formatting);
        }

        public static byte[] ToJsonByteArray<T>(this T source, Encoding encoding,
            Formatting formatting = Formatting.None)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            return (source).ToJObject().ToString(formatting)
                .Encode(encoding);
        }

        public static async Task<byte[]> ToJsonByteArrayAsync<T>(this T source, Encoding encoding,
            Formatting formatting = Formatting.None)
        {
            return await Task.Run(() => source.ToJsonByteArray(encoding, formatting)).ConfigureAwait(false);
        }
    }
}