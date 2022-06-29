// ReSharper disable UnusedMember.Global

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tools.Belt.Common.Extensions
{
    public static class NewtonsoftExtensions
    {
        /// <summary>
        ///     Converts a <see cref="JObject" /> to a <see cref="JToken" />
        /// </summary>
        /// <param name="source">The <see cref="JObject" /> to convert</param>
        /// <returns>The converted <see cref="JToken" /></returns>
        public static JObject ToJObject(this JToken source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source as JObject;
        }

        /// <summary>
        ///     Performs a <see cref="JArray" />.Parse(str) call to convert a json string to an <see cref="JArray" /> instance
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}" /> to parse</param>
        /// <returns>The parsed <see cref="JArray" /></returns>
        [ExcludeFromCodeCoverage]
        public static JArray ToJArray(this IEnumerable source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            JArray result = new JArray();
            foreach (object item in source) result.Add(item.ToJToken());
            return result;
        }

        /// <summary>
        ///     Converts a <see cref="T" /> to a <see cref="JObject" />
        /// </summary>
        /// <param name="source">The <see cref="T" /> to convert</param>
        /// <returns>The converted <see cref="JObject" /></returns>
        public static JObject ToJObject<T>(this T source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return JObject.FromObject(source);
        }

        /// <summary>
        ///     Converts a <see cref="JObject" /> to a <see cref="T" />
        /// </summary>
        /// <param name="source">The <see cref="T" /> to convert</param>
        /// <returns>The converted <see cref="JObject" /></returns>
        public static async Task<JObject> ToJObjectAsync<T>(this T source)
        {
            return await Task.Run(() => source.ToJObject()).ConfigureAwait(false);
        }

        /// <summary>
        ///     Converts an <see cref="IEnumerable{T}" /> to a <see cref="JToken" /> by calling <seealso cref="ToJArray" />
        /// </summary>
        /// <typeparam name="T">The Type of <see cref="T" /></typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /></param>
        /// <returns>a <see cref="JToken" /></returns>
        [ExcludeFromCodeCoverage]
        public static JToken ToJToken<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.ToJArray();
        }

        /// <summary>
        ///     Converts an <see cref="T" /> safely to a <see cref="JToken" /> by calling <seealso cref="ToJObject" /> within a
        ///     try/catch block
        /// </summary>
        /// <typeparam name="T">The Type of <see cref="T" /></typeparam>
        /// <param name="source">The instance of <see cref="T" /></param>
        /// <param name="logger">The logger to be used</param>
        /// <returns>a <see cref="JToken" /></returns>
        [ExcludeFromCodeCoverage]
        public static JToken ToSafeJToken<T>(this T source, ILogger logger)
        {
            try
            {
                if (source is JToken token) return token;
                return source.ToJToken();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                logger.LogTrace(
                    $"Unable to jsonify the object {source}, will return null instead, this was due to: {e.FullStackTrace()}");
                return new JValue((object) null);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        ///     Converts an <see cref="T" /> to a <see cref="JToken" /> by calling <seealso cref="ToJObject" />
        /// </summary>
        /// <typeparam name="T">The Type of <see cref="T" /></typeparam>
        /// <param name="source">The instance of <see cref="T" /></param>
        /// <returns>a <see cref="JToken" /></returns>
        [ExcludeFromCodeCoverage]
        public static JToken ToJToken<T>(this T source)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (source == null) return new JValue((object) null);
            if (source is string) return new JValue(source);
            if (source is IEnumerable enumerable) return enumerable.ToJArray();

            if (source.GetType().IsPrimitive) return new JValue(source);
            return source.ToJObject();
        }

        /// <summary>
        ///     Converts a json <see cref="string" /> to a <see cref="T" />
        /// </summary>
        /// <typeparam name="T">The object to deserialize json to</typeparam>
        /// <param name="source">The json string to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T ReadJsonStringToObject<T>(this string source)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            return JsonConvert.DeserializeObject<T>(source);
        }

        /// <summary>
        ///     Converts a <see cref="JObject" /> to a <see cref="string" />
        /// </summary>
        /// <param name="source">The source <see cref="JObject" /></param>
        /// <param name="formatting">The <see cref="Formatting" /> to apply (defaults to Formatting.Indented)</param>
        /// <returns>The Json formatted <see cref="string" /></returns>
        public static string ToJson(this JToken source, Formatting formatting = Formatting.Indented)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.ToString(formatting);
        }

        /// <summary>
        ///     Converts a <see cref="T" /> to a <see cref="string" />
        /// </summary>
        /// <param name="source">The source <see cref="T" /></param>
        /// <param name="formatting">The <see cref="Formatting" /> to apply (defaults to Formatting.Indented)</param>
        /// <returns>The Json formatted <see cref="string" /></returns>
        public static string ToJson<T>(this T source, Formatting formatting = Formatting.Indented)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.ToJToken().ToJson(formatting);
        }

        /// <summary>
        ///     Converts the value of a <see cref="JToken" /> to a nullable boolean by chaining a call to
        ///     <see cref="SafeToString" /> and <see cref="StringExtensions.SafeToBoolean" />
        /// </summary>
        /// <param name="source">The <see cref="JToken" /> to convert</param>
        /// <returns>The converted <see cref="Nullable" /> boolean</returns>
        public static bool? ToBoolean(this JToken source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.SafeToString().ToNullableBoolean();
        }

        /// <summary>
        ///     Converts the value of a <see cref="JToken" /> to a nullable datetime by chaining a call to
        ///     <see cref="SafeToString" /> and
        ///     <see cref="StringExtensions.ToDateTime(string, string, System.Globalization.DateTimeStyles)" />
        /// </summary>
        /// <param name="source">The <see cref="JToken" /> to convert</param>
        /// <returns>The converted <see cref="Nullable" /> DateTime</returns>
        public static DateTime? ToDateTime(this JToken source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.SafeToString().ToDateTime();
        }

        /// <summary>
        ///     Converts the value of a <see cref="JToken" /> to a string, or null if token is null or token type is null
        /// </summary>
        /// <param name="source">The <see cref="JToken" /> to convert</param>
        /// <returns>The string or null</returns>
        public static string SafeToString(this JToken source)
        {
            return source == null || source.Type == JTokenType.Null ? null : source.ToString();
        }

        /// <summary>
        ///     Safely reads the direct child token with the specified name from <see cref="JToken" />
        /// </summary>
        /// <param name="source">The <see cref="JToken" /> to read from</param>
        /// <param name="tokenName">The name of the child token to read</param>
        /// <returns>The <see cref="JToken" /> if found, or null otherwise</returns>
        public static JToken ChildToken(this JToken source, string tokenName)
        {
            return source?[tokenName];
        }

        /// <summary>
        ///     Tries to read the name of the <see cref="JToken" /> if possible, returns null otherwise
        /// </summary>
        /// <param name="source">The <see cref="JToken" /> to read from</param>
        /// <returns>The <see cref="JObject" /> Name property value if the JToken is of <see cref="JObject" /> type, null otherwise</returns>
        public static string ReadName(this JToken source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            switch (source.Type)
            {
                case JTokenType.Property:
                    return ((JProperty) source).Name;
                case JTokenType.Object:
                    List<JProperty> properties = source.ToJObject()?.Properties()?.ToList() ?? new List<JProperty>();
                    return properties.Count == 1 ? properties.SingleOrDefault()?.Name : null;
                case JTokenType.Constructor:
                case JTokenType.Comment:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                case JTokenType.Array:
                case JTokenType.None:
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Hierarchically navigate through the children until a matching token is found, returning it, or null if not found/
        /// </summary>
        /// <param name="source">The source <see cref="JToken" /> where to search for the child</param>
        /// <param name="tokenName">The name of the token to search for</param>
        /// <returns>The first child token with the requested name, or null if none found</returns>
        public static JToken FindToken(this JToken source, string tokenName)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            // ReSharper disable ConstantConditionalAccessQualifier
            if ((!(source is JValue) && !(source is JProperty) && source[tokenName] != null  ) || ( !(source is JValue)  && !(source?.Children().Any()).GetValueOrDefault()))
                return source?[tokenName];
            // ReSharper restore ConstantConditionalAccessQualifier

            foreach (JToken child in source.Children())
            {
                JToken result = child.FindToken(tokenName);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        ///     Dynamically add new properties to a <see cref="JObject" />
        /// </summary>
        /// <param name="source">The <see cref="JObject" /> to add properties to</param>
        /// <param name="properties">The list of properties to be added in a <see cref="KeyValuePair{TKey,TValue}" /> format</param>
        /// <returns>The <see cref="source" /> <see cref="JObject" /> for fluent syntax.</returns>
        public static JObject AddProperties(this JObject source, params KeyValuePair<string, JToken>[] properties)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (KeyValuePair<string, JToken> property in properties) source.Add(property.Key, property.Value);
            return source;
        }

        /// <summary>
        ///     Hierarchically navigate through the children returning all matching tokens
        /// </summary>
        /// <param name="source">The source <see cref="JToken" /> where to search for the child</param>
        /// <param name="tokenName">The name of the token to search for</param>
        /// <returns>All the child tokens with the requested name</returns>
        public static IEnumerable<JToken> FindTokens(this JToken source, string tokenName)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            List<JToken> result = new List<JToken>();

            if (source.ReadName() == tokenName &&
                source.Path.EndsWith($".{tokenName}", StringComparison.OrdinalIgnoreCase))
            {
                Trace.TraceInformation($"Adding Node with Path: {source.Path}");
                result.Add(source);
            }

            foreach (JToken child in source.Children()) result.AddRange(child.FindTokens(tokenName));

            return result;
        }
    }
}