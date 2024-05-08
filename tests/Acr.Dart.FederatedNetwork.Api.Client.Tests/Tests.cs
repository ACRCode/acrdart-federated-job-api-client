using Acr.Dart.FederatedNetwork.Api.Client.Abstractions;
using Acr.Dart.FederatedNetwork.Api.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Acr.Dart.FederatedNetwork.Api.Client.Clients;

namespace AILab.Rhino.Api.Client.Tests
{
    public class RhinoTests
    {

        [NotNull] private IConfiguration _configuration;
        [NotNull] private IServiceProvider _serviceProvider;
        [NotNull] private IFederatedNetworkClientFactory _fnClientFactory;


        [SetUp]
        public void Setup()
        {
            //create configuration
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _configuration = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile($"appsettings.{environment}.json", true)
                                    .AddEnvironmentVariables()
                                    .Build();

            //create IOC
            IServiceCollection services = new ServiceCollection();
            var dartBaseUrl = _configuration.GetValue<string>("DartBaseUrl");
            services.AddFederatedNetworkClient( new Uri(dartBaseUrl));
            _serviceProvider = services.BuildServiceProvider();
            _fnClientFactory = _serviceProvider?.GetService<IFederatedNetworkClientFactory>();
        }

        [Test]
        public async Task TestStatus()
        {
            var cancellationToken = CancellationToken.None;
            var dartClient = await _fnClientFactory.CreateAsync<FederatedNetworkClient>(cancellationToken).ConfigureAwait(false);
            var oktaToken = _configuration.GetValue<string>("AuthToken");
            var connectNode = _configuration.GetTestConnectNode();
            var jobId = _configuration.GetTestJobId();


            await dartClient.UpdateFederatedJobLogs(jobId, $"These are some testing logs {DateTime.Now.ToString()}", oktaToken, cancellationToken);

            await dartClient.UpdateFederatedJobStatus(jobId, Acr.Dart.FederatedNetwork.Api.Contract.Enums.FederatedJobStatus.Completed, oktaToken, cancellationToken);

            var jobs = await dartClient.GetFederatedJobsForSite(connectNode, oktaToken);

            Assert.NotNull(jobs);
        }


    }
}