using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tools.Belt.Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static string ReadEmbeddedResource(this Assembly source, string resourceName)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (resourceName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(resourceName));
            Stream sourceStream = source.GetManifestResourceStream(resourceName);

            if (sourceStream == null)
                throw new ArgumentException(GenerateInvalidResourceExceptionMessage(source, resourceName),
                    resourceName);

            return sourceStream.ReadToEnd();
        }

        public static string GetVersionControlString(this Assembly source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            
            string version = source.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            string project = source.GetName().Name;
            string versionControlString = $"{project} v{version}";

            return versionControlString;
        }

        private static string GenerateInvalidResourceExceptionMessage(this Assembly source, string resourceName)
        {
            string message =
                $@"Provided resource name:{System.Environment.NewLine}";

            message += $"\t({resourceName}) is not found in assembly:{System.Environment.NewLine}";
            message += $"\t\t{source.FullName}{System.Environment.NewLine}";
            message += $"Valid options are: {System.Environment.NewLine}";

            return source.GetManifestResourceNames().Aggregate(message,
                (current, manifestResourceName) => current + $"\t{manifestResourceName}{System.Environment.NewLine}");
        }
    }
}