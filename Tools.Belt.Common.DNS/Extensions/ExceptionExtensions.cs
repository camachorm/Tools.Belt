using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Tools.Belt.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static string FullStackTrace(this Exception source, int indentation = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            string stackTrace = $"{source.Message}{System.Environment.NewLine}".Indent(indentation);
            stackTrace += $"{source.StackTrace}{System.Environment.NewLine}".Indent(indentation);

            stackTrace += ProcessWebException(source as WebException, indentation);

            if (source.InnerException == null) return stackTrace;

            stackTrace += "Inner Exception:".Indent(indentation);
            stackTrace += source.InnerException.FullStackTrace(++indentation);

            return stackTrace;
        }

        /// <summary>Flatten an exception and its inner exceptions and return each of their 'Message' values.</summary>
        [ExcludeFromCodeCoverage]
        public static IEnumerable<string> Messages(this Exception ex)
        {
            List<string> result = ex.GetInnerExceptions().Select(e => e.Message).ToList();
            result.Insert(0, ex.Message);
            return result;
        }

        /// <summary>Flatten an exception and its inner exceptions.</summary>
        [ExcludeFromCodeCoverage]
        public static IEnumerable<Exception> GetInnerExceptions(this Exception ex)
        {
            Exception inner = ex ?? throw new ArgumentNullException(nameof(ex));
            return Enumerate();

            IEnumerable<Exception> Enumerate()
            {
                do
                {
                    yield return inner;
                    inner = inner.InnerException;
                } while (inner != null);
            }
        }

        private static string ProcessWebException(Exception e, int indentation = 0)
        {
            if (!(e is WebException)) return "";

            WebException we = (WebException) e;

            string result = $"Web Exception Details: {System.Environment.NewLine}".Indent(indentation);

            result +=
                $"Headers:{System.Environment.NewLine}{we.Response?.Headers?.ToPrintableString() ?? "No Headers found"}{System.Environment.NewLine}"
                    .Indent(indentation + 1);

            return result;
        }

        /// <summary>
        ///     Formats the Exception as a <see cref="JToken" /> with fields: Status, Message, StackTrace
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to format</param>
        /// <returns>The formatted <see cref="JObject" /></returns>
        public static JToken ToErrorJToken(this Exception e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            return new
            {
                Status = "Error",
                e.Message,
                StackTrace = e.FullStackTrace()
            }.ToJToken();
        }
    }
}