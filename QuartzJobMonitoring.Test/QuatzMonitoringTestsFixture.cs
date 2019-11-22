namespace QuartzJobMonitoring.Test
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using System.Net.Http;

    public class QuatzMonitoringTestsFixture
    {
        private readonly TestServer _testServer;
        public readonly HttpClient _client;

        public string Prefix { get; set; }
        public string JobName { get; set; }
        public string LastJobId { get; set; }

        public QuatzMonitoringTestsFixture()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            _client = _testServer.CreateClient();
        }
    }
}
