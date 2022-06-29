// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Versioning;

namespace Tools.Belt.Common.Environment
{
    /// <summary>
    ///     This class provides static standardized access to information about the version of .Net being used at runtime
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class RuntimeInformation
    {
        private const string VersionPrefix = "Version=v";

        private static readonly string FrameworkName =
            Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

        private static readonly int VersionPrefixLength = VersionPrefix.Length;

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static string Version =
#pragma warning restore CA2211 // Non-constant fields should not be visible
            FrameworkName.Substring(
                FrameworkName.IndexOf(VersionPrefix, StringComparison.InvariantCulture) +
                VersionPrefixLength);

        public static bool IsDotNetFramework =>
            FrameworkName.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase);

        public static bool IsDotNetCore => FrameworkName.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase);

        public static bool IsDotNetStandard =>
            FrameworkName.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase);
    }
}