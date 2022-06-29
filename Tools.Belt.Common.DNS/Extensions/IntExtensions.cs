using System;

namespace Tools.Belt.Common.Extensions
{
    public static class IntExtensions
    {
        public static bool IsSuccessfulStatusCode(this int source)
        {
            return source >= 200 && source < 300;
        }

        public static int Limit(this int source, int min, int max)
        {
            return Math.Min(Math.Max(source, min), max);
        }
    }
}