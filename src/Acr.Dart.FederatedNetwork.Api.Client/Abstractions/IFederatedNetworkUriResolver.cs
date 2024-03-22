using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Acr.Dart.FederatedNetwork.Api.Client.Abstractions
{
    /// <summary>
    /// An abstraction for URI resolution process.
    /// </summary>
    [PublicAPI]
    public interface IFederatedNetworkUriResolver
    {
        /// <summary>
        /// Gets FederatedNetwork API base URI.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to cancel this operation.</param>
        /// <returns>An asynchronous task which will get resolved to the Dart Api URI.</returns>
        [NotNull, ItemCanBeNull]
        Task<Uri> GetFederatedNetworkUriAsync(CancellationToken cancellationToken);
    }
}
