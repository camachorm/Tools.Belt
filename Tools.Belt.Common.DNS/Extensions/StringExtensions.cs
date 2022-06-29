// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Inserts a separator before every capital letter in a string. Also calls Trim on the string.
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <returns>The separated string.</returns>
        public static string SplitByCamelCase(this string source, string separator)
        {
            return Regex.Replace(source, "([A-Z])", $"{separator}$1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Encrypts string using AES and Base64.
        /// </summary>
        /// <param name="source">String to encrypt.</param>
        /// <param name="key">16 character password.</param>
        /// <param name="IV">16 charcater initialization vector. 
        /// Should be different between each encrypted object, but stored to allow decryption.</param>
        /// <returns>Ecrypted string.</returns>
        public static string Encrypt(this string source, string key, string IV)
        {
            // Check arguments.
            if (source == null || source.Length <= 0)
                throw new ArgumentNullException(nameof(source));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));
            string encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.ASCII.GetBytes(key);
                aesAlg.IV = Encoding.ASCII.GetBytes(IV);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(source);
                        }
                        encrypted = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// Decrypts string using AES and Base64.
        /// </summary>
        /// <param name="source">String to decrypt.</param>
        /// <param name="key">Password.</param>
        /// <param name="IV">Initialization vector.</param>
        /// <returns>Decrypted string.</returns>
        public static string Decrypt(this string source, string key, string IV)
        {
            // Check arguments.
            if (source == null || source.Length <= 0)
                throw new ArgumentNullException(nameof(source));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));
            string decrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.ASCII.GetBytes(key);
                aesAlg.IV = Encoding.ASCII.GetBytes(IV);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(source)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            decrypted = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            // Return the decrypted bytes from the memory stream.
            return decrypted;
        }

        /// <summary>
        /// Encodes string in base64.
        /// </summary>
        /// <param name="source">Plain text string.</param>
        /// <returns>Encoded string.</returns>
        public static string ToBase64(this string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            string result = Convert.ToBase64String(bytes);
            return result;
        }

        /// <summary>
        /// Converts base64 encoded string.
        /// </summary>
        /// <param name="source">Base64 string.</param>
        /// <returns>Deserialized object.</returns>
        public static string FromBase64(this string source)
        {
            byte[] bytes = Convert.FromBase64String(source);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        /// <summary>
        /// Deserializes base64 encoded json string into an object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="source">Base64 json string.</param>
        /// <returns>Deserialized object.</returns>
        public static T FromJsonBase64<T>(this string source)
        {
            byte[] bytes = Convert.FromBase64String(source);
            string json = Encoding.Default.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        ///     Returns the result of string.IsNullOrEmpty(source) for more fluent code writing
        /// </summary>
        /// <param name="source">The string to check if its null or empty</param>
        /// <returns>True or false</returns>
        [ExcludeFromCodeCoverage] // Exclusion Reason: Usage of native code only
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }


#pragma warning disable CA1200 // Avoid using cref tags with a prefix
        /// <summary>
        ///     Converts the provided string into its UTF-8 Encoded binary representation.
        /// </summary>
        /// <param name="source">The <see cref="string" /> to convert.</param>
        /// <returns>The encoded <see cref="T:byte[]" /></returns>
        [ExcludeFromCodeCoverage] // Exclusion Reason: Usage of native code only
#pragma warning restore CA1200 // Avoid using cref tags with a prefix
        public static byte[] GetBytes(this string source)
        {
            return source.GetBytes(Encoding.UTF8);
        }

#pragma warning disable CA1200 // Avoid using cref tags with a prefix
        /// <summary>
        ///     Converts the provided string into its Encoded binary representation using the provided <see cref="Encoding" />
        ///     instance's GetBytes method.
        /// </summary>
        /// <param name="source">The <see cref="string" /> to convert.</param>
        /// <param name="encoding">The <see cref="Encoding" /> instance to be used.</param>
        /// <returns>The encoded <see cref="T:byte[]" />.</returns>
#pragma warning restore CA1200 // Avoid using cref tags with a prefix
        public static byte[] GetBytes(this string source, Encoding encoding)
        {
            return (encoding ?? throw new ArgumentNullException(nameof(encoding))).GetBytes(
                source ?? throw new ArgumentNullException(nameof(source)));
        }

        /// <summary>
        ///     Removes the Configuration Provider name from a string generated by the <see cref="IConfigurationService.Keys" />
        ///     property so we can use the key itself
        /// </summary>
        /// <param name="source">The generated entry in the format 'SomeProviderName:: SomeKeyName'</param>
        /// <returns>The filtered out 'SomeKeyName'</returns>
        public static string FilterOutIConfigurationProvider(this string source)
        {
            const string marker = ":: ";
            if (source.IsNullOrEmpty()) return source;
            int indexOf = source.IndexOf(marker, StringComparison.Ordinal) + marker.Length;
            return source.Substring(indexOf);
        }

        /// <summary>
        ///     Returns the result of string.IsNullOrWhiteSpace(source) for more fluent code writing
        /// </summary>
        /// <param name="source">The string to check if its null or whitespace</param>
        /// <returns>True or false</returns>
        [ExcludeFromCodeCoverage] // Exclusion Reason: Usage of native code only
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        ///     Concatenates <paramref name="segment" /> to <paramref name="source" />, adding a Uri separator char ('/') if
        ///     necessary in between
        /// </summary>
        /// <param name="source">
        ///     The original string to which we want to append a Uri segment.
        ///     e.g. - https://login.microsoftonline.com
        /// </param>
        /// <param name="segment">
        ///     The segment to be appended
        ///     e.g. - this_is_my_tenant_id
        /// </param>
        /// <returns>
        ///     The concatenated string
        ///     e.g. - https://login.microsoftonline.com/this_is_my_tenant_id
        /// </returns>
        /// <exception cref="UriFormatException">If source is not a valid Uri</exception>
#pragma warning disable CA1055 // Uri return values should not be strings
        public static string AddUriSegment(this string source, string segment)
#pragma warning restore CA1055 // Uri return values should not be strings
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            if (!source.IsValidUri())
                throw new UriFormatException($"The provided string ({source}) is not a valid Uri");
            if (!source.EndsWithDirectorySeparatorChar())
                source += '/';

            return source + segment;
        }

        public static string AddUriSegment(this Uri source, string segment)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.AbsoluteUri.AddUriSegment(segment);
        }

        /// <summary>
        ///     Checks if the provided string ends with a directory separator char
        /// </summary>
        /// <param name="source">The string to check</param>
        /// <returns>True if it ends with either Path.DirectorySeparatorChar or Path.AltDirectorySeparatorChar</returns>
        public static bool EndsWithDirectorySeparatorChar(this string source)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            return !source.IsNullOrEmpty() &&
                   (source.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture),
                        StringComparison.OrdinalIgnoreCase) ||
                    source.EndsWith(Path.AltDirectorySeparatorChar.ToString(CultureInfo.CurrentCulture),
                        StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Performs an inline validation if a string is a valid Uri by attempting Uri.TryCreate with UriKind.Absolute
        /// </summary>
        /// <param name="source">The string to validate</param>
        /// <param name="type">The UriKind value to use, defaults as UriKind.Absolute</param>
        /// <returns>True or False</returns>
        [ExcludeFromCodeCoverage] // Exclusion Reason: Usage of native code only
        public static bool IsValidUri(this string source, UriKind type = UriKind.Absolute)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            // ReSharper disable UnusedVariable
            return Uri.TryCreate(source, type, out Uri ignored);
            // ReSharper restore UnusedVariable
        }

        /// <summary>
        ///     Performs a XElement.Parse(source) call to convert a string to an XElement instance
        /// </summary>
        /// <param name="str">The xml string to parse</param>
        /// <returns>The parsed XElement</returns>
        [ExcludeFromCodeCoverage] // Exclusion Reason: Usage of native code only
        public static XElement ToXElement(this string str)
        {
            return str.IsNullOrEmpty() ? null : XElement.Parse(str);
        }

        /// <summary>
        ///     Performs a JObject.Parse(source) call to convert a json string to an JObject instance
        /// </summary>
        /// <param name="str">The xml string to parse</param>
        /// <returns>The parsed JObject</returns>
        [ExcludeFromCodeCoverage]
        public static JObject ToJObject(this string str)
        {
            return str.IsNullOrEmpty() ? null : JObject.Parse(str);
        }

        /// <summary>
        ///     Performs a <see cref="JArray" />.Parse(source) call to convert a json string to an <see cref="JArray" /> instance
        /// </summary>
        /// <param name="source">The xml string to parse</param>
        /// <returns>The parsed <see cref="JArray" /></returns>
        [ExcludeFromCodeCoverage]
        public static JArray ToJArray(this string source)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            return JArray.Parse(source);
        }

        /// <summary>
        ///     Checks if the string is a <see cref="JArray" />, if so uses <code>JArray.Parse</code> to convert to a valid
        ///     <see cref="JToken" />
        ///     Otherwise calls <see cref="ToJObject" /> to trickle down the call
        /// </summary>
        /// <param name="source">The xml string to parse</param>
        /// <returns>
        ///     The parsed <see cref="JToken" /> which can be either a <see cref="JArray" /> or a <see cref="JObject" />
        /// </returns>
        [ExcludeFromCodeCoverage]
        public static JToken ParseJson(this string source)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            return source.StartsWith("[", StringComparison.InvariantCulture)
                ? (JToken) source.ToJArray()
                : source.ToJObject();
        }

        /// <summary>
        ///     Performs a .ToXElement().ToJObject() call to convert a string to an JObject instance
        /// </summary>
        /// <param name="source">The xml string to parse</param>
        /// <returns>The parsed JObject</returns>
        [ExcludeFromCodeCoverage]
        public static JObject ConvertXElementToJObject(this string source)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            return source.ToXElement().ToJObject();
        }

        /// <summary>
        ///     Loads a <see cref="CultureInfo" /> object for the given <see cref="cultureInfo" />, using <see cref="style" />.
        ///     If no value for <see cref="cultureInfo" /> can be found, it defaults to the current culture
        /// </summary>
        /// <param name="source">The string to try to parse into a <see cref="DateTime" /></param>
        /// <param name="cultureInfo">The cultureInfo to use</param>
        /// <param name="style">The <see cref="DateTimeStyles" /> to use, defaulting to <see cref="DateTimeStyles.None" /></param>
        /// <returns>The parsed <see cref="DateTime" /> value or null if it fails to parse</returns>
        public static DateTime? ToDateTime(this string source, string cultureInfo = null,
            DateTimeStyles style = DateTimeStyles.None)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            CultureInfo culture = new CultureInfo(cultureInfo ?? CultureInfo.CurrentCulture.Name, false);
            if (DateTime.TryParse(source, culture, style, out DateTime result))
                return result;

            return null;
        }

        /// <summary>
        ///     Indents a string by prepending a tab character the specified number of times <see cref="indentation" />
        /// </summary>
        /// <param name="source">The source string to be indented</param>
        /// <param name="indentation">The indentation level to apply (has to be greater or equal to 0)</param>
        /// <returns>The indented string</returns>
        public static string Indent(this string source, int indentation = 0)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            if (indentation < 0)
                // ReSharper disable LocalizableElement
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new ArgumentOutOfRangeException(nameof(indentation),
                    "Indentation level has to be greater or equal to 0.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            // ReSharper restore LocalizableElement

            return $"{new string('\t', indentation)}{source}";
        }

        /// <summary>
        ///     Converts a string to a bool? by normalizing its contents based on general acceptance/rejection statements:
        ///     ok, yes, true = true
        ///     cancel, no, false = false
        ///     anything else = null
        /// </summary>
        /// <param name="source">The string to be converted.</param>
        /// <returns>True, False or null accordingly</returns>
        public static bool SafeToBoolean(this string source)
        {
            return source.ToNullableBoolean().GetValueOrDefault(false);
        }

        /// <summary>
        ///     Converts a string to a Guid using Guid.Parse().
        ///     Method provided for consistency.
        /// </summary>
        /// <param name="source">The string to convert.</param>
        /// <returns>The Guid.</returns>
        public static Guid ToGuid(this string source)
        {
            return Guid.Parse(source);
        }

        /// <summary>
        ///     Converts a string to a SecureString.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            SecureString result = new SecureString();
            foreach (char character in source) result.AppendChar(character);
            return result;
        }

        /// <summary>
        ///     Converts a string to a bool? by normalizing its contents based on general acceptance/rejection statements:
        ///     ok, yes, true = true
        ///     cancel, no, false = false
        ///     anything else = null
        /// </summary>
        /// <param name="source">The string to be converted.</param>
        /// <returns>True, False or null accordingly</returns>
        public static bool? ToNullableBoolean(this string source)
        {
            source ??= "false";

            // TODO: Add Configuration built in support for these value lists
            IList<string> agreements = new List<string>
            {
                "ok",
                "yes",
                "true"
            };

            IList<string> rejections = new List<string>
            {
                "cancel",
                "no",
                "false"
            };

            return source.ToNullableBoolean(agreements, rejections);
        }

        /// <summary>
        ///     Converts a string to a bool? by normalizing its contents based on a list of valid statements for either condition.
        ///     Returns null if neither is found
        /// </summary>
        /// <param name="source">The string to be converted.</param>
        /// <param name="agreements">The list of values to map to true</param>
        /// <param name="rejections">The list of values to map to false</param>
        /// <returns>True, False or null accordingly</returns>
        public static bool? ToNullableBoolean(this string source, IList<string> agreements, IList<string> rejections)
        {
            const string trueStatement = "true";
            const string falseStatement = "false";

            if (source.IsNullOrEmpty()) return null;

            if (agreements == null) throw new ArgumentNullException(nameof(agreements));
            if (rejections == null) throw new ArgumentNullException(nameof(rejections));


            if (bool.TryParse(source, out bool result)) return result;

            if (agreements.Any(a =>
                a.ToUpperInvariant().Equals(source.ToUpperInvariant(), StringComparison.InvariantCulture)))
                source = trueStatement;

            if (rejections.Any(a =>
                a.ToUpperInvariant().Equals(source.ToUpperInvariant(), StringComparison.InvariantCulture)))
                source = falseStatement;

            if (!bool.TryParse(source, out result)) return null;

            return result;
        }

        public static string ToPrintableSecret(this string source, int percentageConst = 5, int maxCharCount = 4)
        {
            if (source.IsNullOrEmpty() || source.Length == 1) return source;

            double length = source.Length;
            double percentageBase = length / 100;
            double percentage = percentageConst * percentageBase;

            int charCount = (int) percentage;

            if (charCount > maxCharCount) charCount = maxCharCount;
            if (charCount <= 0) charCount = 1;

            while (length <= charCount * 2 && charCount > 1) charCount--;

            string result = $"{source.Substring(0, charCount)}*****";

            if (length > charCount * 2)
                result = $"{result}{source.Substring((int) length - charCount)}";

            return result;
        }

        public static Uri ToUri(this string source)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            return new Uri(source);
        }

        /// <summary>
        ///     Converts a string to an int value
        /// </summary>
        /// <param name="source">The input string</param>
        /// <returns>An int value</returns>
        public static int? ToInt(this string source)
        {
            if (int.TryParse(source, out int output))
                return output;

            return null;
        }

        /// <summary>
        ///     Converts a string to an Nullable Decimal value
        /// </summary>
        /// <param name="source">The input string</param>
        /// <returns>An Decimal value</returns>
        public static decimal? ToDecimal(this string source)
        {
            if (decimal.TryParse(source, out decimal output))
                return output;

            return null;
        }

        /// <summary>
        ///     Converts a string to an Nullable long value
        /// </summary>
        /// <param name="source">The input string</param>
        /// <returns>An long value</returns>
        public static long? ToLong(this string source)
        {
            if (long.TryParse(source, out long output))
                return output;

            return null;
        }

        /// <summary>
        ///     Converts a string to an Decimal value
        /// </summary>
        /// <param name="source">The input string</param>
        /// <param name="defaultValue">(Optional) the default value to return in case it is null</param>
        /// <returns>An Decimal value</returns>
        public static decimal SafeToDecimal(this string source, decimal defaultValue = decimal.MinusOne)
        {
            return source.ToDecimal().GetValueOrDefault(defaultValue);
        }

        /// <summary>
        ///     Converts a string to an long value
        /// </summary>
        /// <param name="source">The input string</param>
        /// <param name="defaultValue">(Optional) the default value to return in case it is null</param>
        /// <returns>A long value</returns>
        public static long SafeToLong(this string source, long defaultValue = long.MinValue)
        {
            return source.ToLong().GetValueOrDefault(defaultValue);
        }

        /// <summary>
        ///     Converts a string to an Nullable Int value
        /// </summary>
        /// <param name="source">The input string</param>
        /// <param name="defaultValue">(Optional) the default value to return in case it is null</param>
        /// <returns>An int value</returns>
        public static int SafeToInt(this string source, int defaultValue = int.MinValue)
        {
            return source.ToInt().GetValueOrDefault(defaultValue);
        }

        /// <summary>
        ///  Converts a string into Timespan
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>A <see cref="TimeSpan" /></returns>
        public static TimeSpan SafeToTimeSpan(this string source, TimeSpan defaultValue)
        {
            if (source == null) return defaultValue;
            bool parsed = TimeSpan.TryParse(source, out TimeSpan parsedResult);
            return parsed ? parsedResult : defaultValue;
        }

        /// <summary>
        ///     Returns the value of a <see cref="string" /> object or, if the source is null, a default value as provided by the
        ///     client
        /// </summary>
        /// <param name="source">The source string to evaluate</param>
        /// <param name="defaultValue">The value to return if <see cref="source" /> is null</param>
        /// <returns>Either <see cref="source" /> if initialized or <see cref="defaultValue" /> if <see cref="source" /> is null.</returns>
        [ExcludeFromCodeCoverage]
        public static string GetValueOrDefault(this string source, string defaultValue)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            if (defaultValue.IsNullOrEmpty()) throw new ArgumentNullException(nameof(defaultValue));
            return source ?? defaultValue;
        }

        /// <summary>
        ///     Attempts to read the blob storage name from <see cref="source" />
        /// </summary>
        /// <param name="source">The connection string to be parsed</param>
        /// <returns>The name if found or null</returns>
        [ExcludeFromCodeCoverage]
        public static string ReadBlobStorageNameFromConnectionString(this string source)
        {
            if (source.IsNullOrEmpty()) throw new ArgumentNullException(nameof(source));
            return ReadKeyValueFromString(source, "AccountName")?.Value;
        }

        /// <summary>
        ///     Attempts to read a key and value from a string using <see cref="key" /> to identify the start of the string,
        ///     <see cref="elementSeparator" /> to identify the end of the string
        ///     and using <see cref="keyValueSeparator" /> to separate the key from the value
        /// </summary>
        /// <param name="source">
        ///     The original string to search from (e.g. -
        ///     'Accept=Application/json;Content-Type=Application/json')
        /// </param>
        /// <param name="key">The key to search for (e.g. - 'Content-Type')</param>
        /// <param name="keyValueSeparator">The separator character that splits key from value (e.g. - '=')</param>
        /// <param name="elementSeparator">The separator character that splits between elements (e.g. - ';')</param>
        /// <returns>
        ///     <see cref="KeyValuePair{TKey,TValue}" /> with the requested <see cref="key" /> and associated value, or null
        ///     if not found.
        /// </returns>
        public static KeyValuePair<string, string>? ReadKeyValueFromString(
            this string source,
            string key,
            char keyValueSeparator = '=',
            char elementSeparator = ';')
        {
            if (source.IsNullOrEmpty()) return null;

            int startElementIndex = source.IndexOf(key, StringComparison.Ordinal);
            if (startElementIndex < 0) return null;

            int endElementIndex = source.IndexOf(elementSeparator, startElementIndex);
            if (endElementIndex < 0) endElementIndex = source.Length;

            string elementContent = source.Substring(startElementIndex, endElementIndex - startElementIndex);

            int separatorIndex = elementContent.IndexOf(keyValueSeparator);
            if (separatorIndex < 0) return null;

            return new KeyValuePair<string, string>(key, elementContent.Substring(separatorIndex + 1));
        }
    }
}