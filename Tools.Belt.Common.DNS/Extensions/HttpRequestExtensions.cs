using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Tools.Belt.Common.Exceptions;

namespace Tools.Belt.Common.Extensions
{
    public static class HttpRequestExtensions
    {
        public delegate T ParseFunction<T>(StringValues a);

        /// <summary>
        /// Get the body of a <see cref="HttpRequest"/> as a class.
        /// </summary>
        /// <param name="req">The request to process.</param>
        /// <returns>The body, converted to a class.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidHttpRequestException"></exception>
        public async static Task<T> GetJsonBodyAsync<T>(this HttpRequest req, bool required = true)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            string body;
            using StreamReader sr = new StreamReader(req.Body);
            {
                body = await sr.ReadToEndAsync().ConfigureAwait(false);
            }
            if (string.IsNullOrEmpty(body) && required) throw new InvalidHttpRequestException($"Body cannot be empty.");

            try
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (JsonException e)
            {
                throw new InvalidHttpRequestException($"Failed to deserialize request. Error: '{e.Message}'", e);
            }
        }

        /// <summary>
        /// Gets a parameter from a <see cref="HttpRequest"/>.
        /// </summary>
        /// <param name="req">The request to process.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="required">If set to true, an <see cref="InvalidHttpRequestException"/> exception is thrown on null.</param>
        /// <returns>The parameter value. Returns default if the value is null and mandatory is set to false.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidHttpRequestException"></exception>
        public static StringValues GetParameter(this HttpRequest req, string name, bool required = true)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            if (req.Query.TryGetValue(name, out StringValues values) == false || string.IsNullOrEmpty(values))
            {
                if (required) throw new InvalidHttpRequestException($"Unable to get '{name}' parameter.");
                return default;
            }

            return values;
        }
        
        /// <summary>
        /// Gets a parameter from a <see cref="HttpRequest"/> using a parse function.
        /// </summary>
        /// <param name="req">The request to process.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="parseFunction">Parse function to parse the value of the parameter.</param>
        /// <param name="required">If set to true, an <see cref="InvalidHttpRequestException"/> exception is thrown on null.</param>
        /// <returns>The parameter value. Returns default if the value is null and mandatory is set to false.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidHttpRequestException"></exception>
        public static T GetParameter<T>(this HttpRequest req, string name, ParseFunction<T> parseFunction, bool required = true)
        {
            if (parseFunction == null) throw new ArgumentNullException(nameof(parseFunction));

            var param = req.GetParameter(name, required);

            try
            {
                if (param != default(StringValues))
                {
                    return parseFunction(param);
                }
                else return default;
            }
            catch (Exception e)
            {
                throw new InvalidHttpRequestException(
                    $"Unable to parse parameter '{name}' with value '{param}'. Error: '{e.Message}'.", e);
            }
        }
        
        /// <summary>
        /// Gets a date range from a <see cref="HttpRequest"/> parameter.
        /// </summary>
        /// <param name="req">The request to process.</param>
        /// <param name="fromName">Name of the 'from' date parameter.</param>
        /// <param name="toName">Name of the 'to' date parameter.</param>
        /// <returns>The date range values.</returns>
        /// <exception cref="InvalidHttpRequestException"></exception>
        public static (DateTime From, DateTime To) GetRequiredParameterUtcDateRange(
            this HttpRequest req,
            string fromName = "from",
            string toName = "to")
        {
            var dates = GetParameterUtcDateRange(req, fromName, toName, true, true);
            return (dates.From.GetValueOrDefault(), dates.To.GetValueOrDefault());
        }
        
        /// <summary>
        /// Gets a date range from a <see cref="HttpRequest"/> parameter.
        /// </summary>
        /// <param name="req">The request to process.</param>
        /// <param name="fromName">Name of the 'from' date parameter.</param>
        /// <param name="toName">Name of the 'to' date parameter.</param>
        /// <param name="fromRequired">If true, an <see cref="InvalidHttpRequestException"/> exception is thrown if from is empty.</param>
        /// <param name="toRequired">If true, an <see cref="InvalidHttpRequestException"/> exception is thrown if to is empty.</param>
        /// <returns>The date range values. Returns null if the value is null and mandatory is set to false.</returns>
        /// <exception cref="InvalidHttpRequestException"></exception>
        public static (DateTime? From, DateTime? To) GetParameterUtcDateRange(
            this HttpRequest req,
            string fromName = "from",
            string toName = "to",
            bool fromRequired = true,
            bool toRequired = true)
        {
            var fromParam = req.GetParameter(fromName, fromRequired);
            var toParam = req.GetParameter(toName, toRequired);
            DateTime? from = null;
            DateTime? to = null;

            try
            {
                if (toParam != default(StringValues)) 
                    to = toParam.ToDateTimeUtc();
            }
            catch (StringValuesConversionException e)
            {
                throw new InvalidHttpRequestException(
                    $"Unable to parse parameter '{toName}' with value '{toParam}'. Error: '{e.Message}'.", e);
            }

            try
            {
                if (fromParam != default(StringValues)) 
                    from = fromParam.ToDateTimeUtc();
            }
            catch (StringValuesConversionException e)
            {
                throw new InvalidHttpRequestException(
                    $"Unable to parse parameter '{fromName}' with value '{fromParam}'. Error: '{e.Message}'.", e);
            }

            if (from > to) throw new InvalidHttpRequestException(
                $"Parameter date '{fromName}' ({from}) cannot come after '{fromName}' ({to}).");

            return (from, to);
        }
    }
}