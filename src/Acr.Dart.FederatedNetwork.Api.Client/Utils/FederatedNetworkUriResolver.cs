
using Acr.Dart.FederatedNetwork.Api.Client.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Acr.Dart.FederatedNetwork.Api.Client;

namespace AILab.FLService.Client.Utils
{
    /// <summary>
    /// Resolves uris
    /// </summary>
    public class FederatedNetworkUriResolver : IFederatedNetworkUriResolver
    {

        #region Fields

        [NotNull] private readonly Uri _uri;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes <see cref="FederatedNetworkUriResolver"/> instance.
        /// </summary>
        /// <param name="uri">Static URI to return.</param>
        public FederatedNetworkUriResolver([NotNull] Uri uri)
        {
            Assert.NotNull(uri, nameof(uri));
            _uri = uri;
        }

        #endregion Constructors

        #region Public Methods

        /// <inheritdoc />
        public Task<Uri> GetFederatedNetworkUriAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_uri);
        }

        #endregion Public Methods
    }
}
