using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Acr.Dart.FederatedNetwork.Api.Client.Clients;

namespace Acr.Dart.FederatedNetwork.Api.Client.Abstractions
{
    /// <summary>
    /// Creates and initializes FederatedNetwork client instances (<see cref="FederatedNetworkClient">).
    /// </summary>
    /// <remarks>This interface was introduced to allow mocking in unit tests.</remarks>
    [PublicAPI]
    public interface IFederatedNetworkClientFactory
    {
        /// <summary>
        /// Creates and initializes <see cref="FederatedNetworkClient"/> instance.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to cancel this operation.</param>
        /// <returns>An asynchronous task which will get resolved to <see cref="FederatedNetworkClient"/> instance.</returns>
        /// <exception cref="InvalidOperationException">when FederatedNetwork API URI cannot be resolved.</exception>
        [NotNull, ItemNotNull]
        Task<TClientType> CreateAsync<TClientType>(CancellationToken cancellationToken = default) where TClientType : BaseClient;
    }
}
