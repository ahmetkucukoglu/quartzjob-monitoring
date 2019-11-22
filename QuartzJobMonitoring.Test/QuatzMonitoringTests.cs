namespace QuartzJobMonitoring.Test
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class QuatzMonitoringTests : IClassFixture<QuatzMonitoringTestsFixture>
    {
        private readonly QuatzMonitoringTestsFixture _fixture;

        public QuatzMonitoringTests(QuatzMonitoringTestsFixture fixture)
        {
            _fixture = fixture;

            _fixture.Prefix = "/quartzjob";
            _fixture.JobName = "TestJob";
        }

        [Fact, Priority(1)]
        public async Task GetJobs()
        {
            var response = await _fixture._client.GetAsync($"{_fixture.Prefix}/api/jobs");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jobs = JsonConvert.DeserializeObject<List<JobResponse>>(responseContent);

            Assert.Single(jobs);
            Assert.Equal(_fixture.JobName, jobs.FirstOrDefault().Name);
        }

        [Fact, Priority(2)]
        public async Task GetSchedules()
        {
            var response = await _fixture._client.GetAsync($"{_fixture.Prefix}/api/schedules?jobName={_fixture.JobName}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var schedules = JsonConvert.DeserializeObject<List<ScheduleResponse>>(responseContent);

            Assert.Single(schedules);
        }

        [Fact, Priority(3)]
        public async Task Trigger()
        {
            var response = await _fixture._client.GetAsync($"/quartzjob/api/triggers?jobName={_fixture.JobName}");

            response.EnsureSuccessStatusCode();
        }

        [Fact, Priority(4)]
        public async Task GetHistories()
        {
            await Task.Delay(5000);

            var response = await _fixture._client.GetAsync($"{_fixture.Prefix}/api/histories?jobName={_fixture.JobName}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var histories = JsonConvert.DeserializeObject<List<HistoryResponse>>(responseContent);

            Assert.Single(histories);

            _fixture.LastJobId = histories.FirstOrDefault().JobId;
        }

        [Fact, Priority(5)]
        public async Task GetLogs()
        {
            var response = await _fixture._client.GetAsync($"{_fixture.Prefix}/api/logs?jobId={_fixture.LastJobId}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var logs = JsonConvert.DeserializeObject<List<LogResponse>>(responseContent);

            Assert.Equal(4, logs.Count);
            Assert.Equal(3, logs.Where((x) => x.Level == "Information").Count());
            Assert.Single(logs.Where((x) => x.Level == "Warning"));
        }

        [Fact, Priority(6)]
        public async Task GetStatistic()
        {
            var response = await _fixture._client.GetAsync($"{_fixture.Prefix}/api/jobs");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jobs = JsonConvert.DeserializeObject<List<JobResponse>>(responseContent);

            Assert.NotNull(jobs.FirstOrDefault().Statistic.Labels);
            Assert.NotNull(jobs.FirstOrDefault().Statistic.Series);
        }
    }
}
