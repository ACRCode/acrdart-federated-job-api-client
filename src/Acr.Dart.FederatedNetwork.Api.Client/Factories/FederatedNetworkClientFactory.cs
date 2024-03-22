using Acr.Dart.FederatedNetwork.Api.Client.Abstractions;
using Acr.Dart.FederatedNetwork.Api.Client.Clients;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Acr.Dart.FederatedNetwork.Api.Client.Factories
{

    /// <inheritdoc cref="IFederatedNetworkClientFactory" />
    /// <summary>
    /// Creates and initializes Federated Network API client instances based on Microsoft's <see cref="IHttpClientFactory"/> service.<br/>
    /// This factory class is a recommended approach to ensure that underlying <see cref="HttpClient"/> instances are used in a proper way.<br/>
    /// </summary>
    [PublicAPI]
    public sealed class FederatedNetworkClientFactory : IFederatedNetworkClientFactory
    {
        #region Constants

        internal const string HttpClientName = "FederatedNetworkClient";

        #endregion Constants

        #region Fields

        [NotNull] private readonly IHttpClientFactory _httpClientFactory;
        [NotNull] private readonly IFederatedNetworkUriResolver _uriResolver;
        [NotNull] private readonly IMemoryCache _memoryCache;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialize <see cref="FederatedNetworkClientFactory"/> instance.
        /// </summary>
        /// <param name="httpClientFactory"><see cref="IHttpClientFactory"/> instance to create underlying <see cref="HttpClient"/> instances.</param>
        /// <param name="uriResolver"><see cref="IFederatedNetworkUriResolver"/> instance to resolve Dart API base URI.</param>
        public FederatedNetworkClientFactory([NotNull] IHttpClientFactory httpClientFactory,
            [NotNull] IMemoryCache memoryCache,
            [NotNull] IFederatedNetworkUriResolver uriResolver)
        {
            Assert.NotNull(httpClientFactory, nameof(httpClientFactory));
            Assert.NotNull(uriResolver, nameof(uriResolver));

            _httpClientFactory = httpClientFactory;
            _uriResolver = uriResolver;
            _memoryCache = memoryCache;
        }

        #endregion Constructors

        #region Public Methods

        /// <inheritdoc />
        public async Task<TClientType> CreateAsync<TClientType>(CancellationToken cancellationToken = default) where TClientType : BaseClient
        {
            var uri = await _uriResolver.GetFederatedNetworkUriAsync(cancellationToken).ConfigureAwait(false);
            var client = _httpClientFactory.CreateClient(HttpClientName);
            client.BaseAddress = uri;
            object[] constParams = { client, _memoryCache };
            var obj = Activator.CreateInstance(typeof(TClientType), constParams);
            return (TClientType)obj;
        }

        #endregion Public Methods
    }
}
