using Microsoft.Extensions.Configuration;

namespace AILab.Rhino.Api.Client.Tests
{
    internal static class ConfigurationExtensions
    {
        public static Guid GetTestJobId(this IConfiguration configuration)
        {
            return configuration.GetValue<Guid>("TestArtifacts:JobId");
        }

        public static string GetTestConnectNode(this IConfiguration configuration)
        {
            return configuration.GetValue<string>("TestArtifacts:ConnectNode");
        }
    }
}
