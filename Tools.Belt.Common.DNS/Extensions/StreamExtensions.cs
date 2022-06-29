using System;
using System.IO;
using System.Threading.Tasks;

namespace Tools.Belt.Common.Extensions
{
    public static class StreamExtensions
    {
        public static string ReadToEnd(this Stream source)
        {
            using StreamReader sr = new StreamReader(source);
            return sr.ReadToEnd();
        }

        public static async Task<string> ReadToEndAsync(this Stream source)
        {
            using StreamReader sr = new StreamReader(source);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        public static byte[] ReadAllBytes(this Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            
            MemoryStream memoryStream = new MemoryStream();

            try
            {
                byte[] buffer = new byte[source.Length]; // 32K buffer for example
                int bytesRead = 0;
                int writeOffset = 0;
                
                while (bytesRead < source.Length && (bytesRead = source.Read(buffer, writeOffset , buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, writeOffset, bytesRead);
                    writeOffset += bytesRead;
                }

                memoryStream.Position = 0;

                return memoryStream.ToArray();
            }
            finally
            {
                memoryStream.Dispose();
            }

            

        }
    }
}