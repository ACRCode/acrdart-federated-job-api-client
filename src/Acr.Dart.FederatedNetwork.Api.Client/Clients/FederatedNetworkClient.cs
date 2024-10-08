﻿using Acr.Dart.FederatedNetwork.Api.Contract;
using Acr.Dart.FederatedNetwork.Api.Contract.Enums;
using AcrConnect.Framework.Api.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acr.Dart.FederatedNetwork.Api.Client.Clients
{
    public class FederatedNetworkClient: BaseClient
    {

        #region Constructors
        /// <summary>
        /// /// Initializes <see cref="FederatedNetworkClient" /> instance.
        /// </summary>
        /// <param name="httpClient"><see cref="HttpClient" /> instance to use for HTTP communication.</param>
        public FederatedNetworkClient(HttpClient httpClient) : base(httpClient) { }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all federatedJobs for a site
        /// </summary>
        /// <param name="cancellationToken">Token to track request cancellation</param>
        /// <returns>The array of all federatedJobs</returns>
        public async Task<IEnumerable<FederatedJob>> GetFederatedJobsForSite(string nodeId, string authToken, CancellationToken cancellationToken = default)
        {
            string apiURL = $"api/federatedjobs?nodeId={nodeId}";
            var results = await GetAsync<IEnumerable<FederatedJob>>(apiURL, authToken, cancellationToken).ConfigureAwait(false);
            return results;
        }


        /// <summary>
        /// Gets the stream to the input zip file for the federated job
        /// </summary>
        /// <param name="cancellationToken">Token to track request cancellation</param>
        /// <returns>The input file stream</returns>
        public async Task<Stream> GetFederatedJobInputStream(
            Guid transactionId, 
            string authToken,
            CancellationToken cancellationToken = default)
        {
            string apiURL = $"api/federatedjobs/{transactionId}";

            if (!string.IsNullOrEmpty(authToken))
                await AuthorizeClient(authToken).ConfigureAwait(false);

            var response = await Client.GetAsync(CreateUri(apiURL), cancellationToken).ConfigureAwait(false);
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
            return response.StatusCode != HttpStatusCode.NotFound
                ? await response.Content.ReadAsStreamAsync().ConfigureAwait(false)
                : null;
        }

        /// <summary>
        /// Updates the status of a a federated job
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="status"></param>
        /// <param name="authToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateFederatedJobStatus(
            Guid transactionId,
            FederatedJobStatus status,
            string authToken,
            CancellationToken cancellationToken = default)
        {
            string apiURL = $"api/federatedjobs/{transactionId}/status";
            if (!string.IsNullOrEmpty(authToken))
                await AuthorizeClient(authToken).ConfigureAwait(false);

            //DART will only recognize string content for the body. Any other method won't work
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject((int)status),
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync(CreateUri(apiURL), jsonContent, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
        }


        /// <summary>
        /// Updates the retrieved study count of a a federated job
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="retrievedStudyCount"></param>
        /// <param name="authToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateRetrievedStudyCount(
            Guid transactionId,
            int retrievedStudyCount,
            string authToken,
            CancellationToken cancellationToken = default)
        {
            string apiURL = $"api/federatedjobs/{transactionId}/retrievedstudycount";
            if (!string.IsNullOrEmpty(authToken))
                await AuthorizeClient(authToken).ConfigureAwait(false);

            //DART will only recognize string content for the body. Any other method won't work
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(retrievedStudyCount),
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync(CreateUri(apiURL), jsonContent, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
        }


        /// <summary>
        /// Updates the logs of a a federated job
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="logs"></param>
        /// <param name="authToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateFederatedJobLogs(
            Guid transactionId,
            string logs,
            string authToken,
            CancellationToken cancellationToken = default)
        {
            string apiURL = $"api/federatedjobs/{transactionId}/logs";
            if (!string.IsNullOrEmpty(authToken))
                await AuthorizeClient(authToken).ConfigureAwait(false);

            using var content = new StringContent(JsonConvert.SerializeObject(logs), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(CreateUri(apiURL), content, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads the results of a federated job
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="status"></param>
        /// <param name="authToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UploadFederatedJobResults(
            Guid transactionId,
            FileStream outputStream,
            string authToken,
            CancellationToken cancellationToken = default)
        {

            Assert.NotNull(outputStream, nameof(outputStream));

            string apiURL = $"api/federatedjobs/{transactionId}/results";
            if (!string.IsNullOrEmpty(authToken))
                await AuthorizeClient(authToken).ConfigureAwait(false);

            HttpContent outputContent = new StreamContent(outputStream);
            outputContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = "output.zip", Name = "outputFile" };
            var formData = new MultipartFormDataContent
            {
                outputContent
            };

            var response = await Client.PostAsync(CreateUri(apiURL), formData, cancellationToken).ConfigureAwait(false);
            outputContent.Dispose();
            await EnsureSuccessfulRequest(response).ConfigureAwait(false);
        }

        #endregion
    }
}
