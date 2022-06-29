using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Tools.Belt.Common.Abstractions.Configuration;
using Tools.Belt.Common.Exceptions;
using Tools.Belt.Common.Extensions;

namespace Tools.Belt.Common.Services.Web
{
    public abstract class HttpServiceBase : IHttpServiceBase
    {
        protected HttpServiceBase() : this(new WebClient())
        {
        }

        protected HttpServiceBase(WebClientWrapper client)
        {
            Client = client;
        }

        protected WebClientWrapper Client { get; set; }

        /// <summary>
        ///     Method used to perform a generic request that returns an object of type <see cref="TOut" />,
        ///     you can customize its behaviour by overriding <see cref="CarryOutRequest{TOut}" /> and injecting
        ///     <see cref="executionFunc" />
        /// </summary>
        /// <typeparam name="TOut">The expected return type of this operation</typeparam>
        /// <param name="logger">The <see cref="ILogger" /> instance to be used for logging.</param>
        /// <param name="configuration">The <see cref="IConfigurationService" />.</param>
        /// <param name="headers">
        ///     The headers to be added to the request. (this will replace any headers within the
        ///     <see cref="WebClientWrapper" />
        /// </param>
        /// <param name="endpointUri">The URI to where we want to send the request.</param>
        /// <param name="executionFunc">
        ///     The delegate function to be executed to carry out the operation against the <see cref="WebClientWrapper" />.
        ///     e.g. - async () => await Client.DownloadStringAsync("messageData")
        /// </param>
        /// <returns>An instance of type <see cref="TOut" /></returns>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "Unnecessary overhead.")]
        public virtual async Task<ToolsBeltHttpResponse<TOut>> GenericRequest<TOut>(
            ILogger logger,
            IConfigurationService configuration,
            IEnumerable<KeyValuePair<string, string>> headers,
            Uri endpointUri,
            Func<TOut> executionFunc)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            try
            {
                return await Policy.Handle<WebException>(
                        ex => new[]
                        {
                            WebExceptionStatus.ConnectFailure,
                            WebExceptionStatus.ConnectionClosed,
                            WebExceptionStatus.Timeout,
                            WebExceptionStatus.RequestCanceled,
                            WebExceptionStatus.ConnectionClosed,
                            WebExceptionStatus.UnknownError
                        }.Contains(ex.Status))
                    .WaitAndRetryAsync(
                        configuration["Tools.Belt.Common.Services.Web.Polly.MaxRetries"]
                            .SafeToInt(3),
                        retryAttempt =>
                            TimeSpan.FromSeconds(
                                configuration[
                                        "Tools.Belt.Common.Services.Web.Polly.SleepBaseInterval"]
                                    .SafeToInt(1) * retryAttempt))
                    .ExecuteAsync(
                        () => Task.FromResult(
                            CarryOutRequest(
                                logger,
                                configuration,
                                headers,
                                endpointUri,
                                executionFunc)))
                    .ConfigureAwait(false);
            }
            catch (WebException e)
            {
                ToolsBeltException exception = new ToolsBeltException("Http request failed irrecoverably", e);

                lock (Client)
                {
                    exception.AssociatedObjects.GetOrAdd("Response", e.Response);
                    exception.AssociatedObjects.GetOrAdd("ResponseHeaders", e.Response?.Headers);
                }

                throw exception;
            }
        }

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "Unnecessary overhead.")]
        public virtual ToolsBeltHttpResponse<TOut> CarryOutRequest<TOut>(
            ILogger logger,
            IConfigurationService configuration,
            IEnumerable<KeyValuePair<string, string>> headers,
            Uri endpointUri,
            Func<TOut> executionFunc)
        {
            if (endpointUri == null) throw new ArgumentNullException(nameof(endpointUri));
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (executionFunc == null) throw new ArgumentNullException(nameof(executionFunc));
            ToolsBeltHttpResponse<TOut> response = new ToolsBeltHttpResponse<TOut>();
            List<KeyValuePair<string, string>> headerList = headers.ToList();
            logger?.LogDebug(
                $"Performing call to {endpointUri.AbsoluteUri}, queryParams: {endpointUri.Query}, headers: {headerList.Count}");

            lock (Client)
            {
                try
                {
                    Client.Headers.Clear();

                    Client.BaseAddress = endpointUri.AbsoluteUri;

                    foreach (KeyValuePair<string, string> header in headerList)
                    {
                        logger?.LogDebug(
                            $"Adding header: {header.Key}/{!header.Value.IsNullOrEmpty()}/{header.Value.Length}");

                        Client.Headers.Add(header.Key, header.Value);
                    }

                    response.ResponseHeaders = Client.ResponseHeaders;
                    response.RequestUri = endpointUri;
                    response.ResponseValue = executionFunc.Invoke();
                }
                catch (Exception e)
                {
                    logger?.LogError(e, e.FullStackTrace());
                    throw;
                }
                finally
                {
                    try
                    {
                        string logEntries = "";
                        List<string> keys = Client?.ResponseHeaders?.Keys.Cast<string>().ToList() ?? new List<string>();
                        foreach (string header in keys)
                            logEntries += $"\tHeader: {header} - {Client?.ResponseHeaders?[header]}\n";

                        logger?.LogDebug($"Listing Response Headers\n{logEntries}");
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                    {
                        logger?.LogDebug($"Error printing response headers in finally block of {GetType()}", e);
                    }
                }

                return response;
            }
        }
    }
}