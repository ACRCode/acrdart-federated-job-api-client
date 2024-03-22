
using AILab.FLService.Client.Utils;
using Acr.Dart.FederatedNetwork.Api.Client.Abstractions;
using Acr.Dart.FederatedNetwork.Api.Client.Factories;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Acr.Dart.FederatedNetwork.Api.Client
{

    /// <summary>
    /// Extensions methods to add Dart API client services to <see cref="IServiceCollection" />.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds FederatedNetwork API discovery services based on a static API URI to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance to enhance with the FederatedNetwork discovery services.</param>
        /// <param name="uri">FederatedNetwork API URI.</param>
        /// <returns>The given <see cref="IServiceCollection"/> instance to support calls chaining.</returns>
        public static IServiceCollection AddFederatedNetworkUri(this IServiceCollection services, [NotNull] Uri uri)
        {
            return services.AddSingleton<IFederatedNetworkUriResolver>(new FederatedNetworkUriResolver(uri));
        }

        /// <summary>
        /// Adds FederatedNetwork API Client services to the <see cref="IServiceCollection"/> and configured a related <see cref="HttpClient"/> to use. This implementation leaves the injection of the credential helper to the client.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance to enhance with the Client services.</param>
        /// <param name="uri">FederatedNetwork API URI.</param>
        /// <returns>The given <see cref="IServiceCollection"/> instance to support calls chaining.</returns>
        public static IServiceCollection AddFederatedNetworkClient(this IServiceCollection services, Uri uri = null)
        {
            if (uri != null)
            {
                services.AddFederatedNetworkUri(uri);
            }

            return services.AddHttpClient(FederatedNetworkClientFactory.HttpClientName)
                .Services
                .AddSingleton<IFederatedNetworkClientFactory, FederatedNetworkClientFactory>();
        }
    }
}
