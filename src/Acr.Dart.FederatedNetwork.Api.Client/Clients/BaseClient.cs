using JetBrains.Annotations;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using static System.FormattableString;

namespace Acr.Dart.FederatedNetwork.Api.Client.Clients
{

    /// <inheritdoc />
    /// <summary>
    /// Base Client for API.
    /// </summary>
    [PublicAPI]
    public abstract class BaseClient
    {

        #region Fields

        [NotNull] private readonly HttpClient _httpClient;
        protected HttpClient Client => _httpClient;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes <see cref="BaseClient" /> instance.
        /// </summary>
        /// <param name="httpClient"><see cref="HttpClient" /> instance to use for HTTP communication.</param>
        protected BaseClient([NotNull] HttpClient httpClient)
        {
            Assert.NotNull(httpClient, nameof(httpClient));
            _httpClient = httpClient;
        }

        #endregion Constructors

        #region Protected Methods


        /// <summary>
        /// Authorizes a client's request header with the passed authentication token
        /// </summary>
        /// <param name="authToken">The authentication token to use for authorization</param>
        protected virtual Task AuthorizeClient(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
                throw new UnauthorizedAccessException();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            return Task.CompletedTask;
        }


        /// <summary>
        /// Standard way of doing a generic get request and obtaining a typed response
        /// </summary>
        /// <typeparam name="TReturnType">The type of the response we should return</typeparam>
        /// <param name="apiUrl">The fully formed relative url with query strings</param>
        /// <param name="authToken">The authorization token</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to cancel this operation.</param>
        /// <returns>The response of the call as the type requested</returns>
        protected async Task<TReturnType> GetAsync<TReturnType>(string apiUrl,
                                                              string authToken = null,
                                                              CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                await AuthorizeClient(authToken).ConfigureAwait(false);
            }
            var response = await _httpClient.GetAsync(CreateUri(apiUrl), cancellationToken).ConfigureAwait(false);
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
            if (typeof(TReturnType) == typeof(string))
            {
                var s = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return (TReturnType)Convert.ChangeType(s, typeof(TReturnType), CultureInfo.InvariantCulture);
            }
            var content = await response.Content.ReadAsAsync<TReturnType>(cancellationToken).ConfigureAwait(false);
            return content;
        }


        /// <summary>
        /// Standard way of doing a generic get request without returning any object
        /// </summary>
        /// <param name="apiUrl">The fully formed relative url with query strings</param>
        /// <param name="authToken">The authorization token</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to cancel this operation.</param>
        /// <returns></returns>
        protected async Task GetAsync(string apiUrl,
                                      string authToken = null,
                                      CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                await AuthorizeClient(authToken).ConfigureAwait(false);
            }
            var response = await _httpClient.GetAsync(CreateUri(apiUrl), cancellationToken).ConfigureAwait(false);
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Standard way of doing a generic get request with returning a stream
        /// </summary>
        /// <param name="apiUrl">The fully formed relative url with query strings</param>
        /// <param name="authToken">The authorization token</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to cancel this operation.</param>
        /// <returns>Stream</returns>
        protected async Task<Stream> GetStreamAsync(string apiUrl,
                                      string authToken = null,
                                      CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                await AuthorizeClient(authToken).ConfigureAwait(false);
            }
            var response = await _httpClient.GetAsync(CreateUri(apiUrl), cancellationToken).ConfigureAwait(false);
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return stream;
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Similar to EnsureSuccessApiResultAsync, but correctly sends the error message as the exception message
        /// </summary>
        /// <param name="message">The response message obtained from an api call</param>
        /// <returns></returns>
        [NotNull]
        protected async static Task EnsureSuccessfulRequest(HttpResponseMessage message)
        {
            if (message == null)
#pragma warning disable CA1303 // Do not pass literals as localized parameters (won't be used elsewhere, supress until we need a constants library)
                throw new Exception("No Response");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            if (message.IsSuccessStatusCode)
                return;

            var error = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (message.StatusCode == HttpStatusCode.NotFound)
                throw new FileNotFoundException(error);

            if (message.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException(error);

            throw new Exception(error);
        }


        /// <summary>
        /// Creates <see cref="Uri"/> from a given string. String may represent any relative or absolute URI.
        /// </summary>
        /// <param name="uri">String to convert to <see cref="Uri"/>.</param>
        /// <returns><see cref="Uri"/> created.</returns>
        [NotNull]
        protected static Uri CreateUri([NotNull] FormattableString uri)
        {
            return new Uri(Invariant(uri), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Creates <see cref="Uri"/> from a given string. String may represent any relative or absolute URI.
        /// </summary>
        /// <param name="uri">String to convert to <see cref="Uri"/>.</param>
        /// <returns><see cref="Uri"/> created.</returns>
        [NotNull]
        protected static Uri CreateUri([NotNull] string uri)
        {
            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }
        #endregion Static Methods
    }
}
