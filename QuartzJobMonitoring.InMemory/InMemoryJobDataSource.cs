namespace QuartzJobMonitoring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InMemoryJobDataSource : IJobDataSource
    {
        private readonly List<JobDataHistoryDocument> histories;
        private readonly List<JobDataLogDocument> logs;

        public InMemoryJobDataSource()
        {
            histories = new List<JobDataHistoryDocument>();
            logs = new List<JobDataLogDocument>();
        }

        public Task Start(JobDataStartDocument document)
        {
            var history = new JobDataHistoryDocument
            {
                JobId = document.JobId,
                JobName = document.JobName,
                JobType = document.JobType,
                StartDate = document.StartDate
            };

            histories.Add(history);

            var description = $"{document.JobName} started executing in {document.StartDate.ToString("dddd, dd MMMM yyyy HH:mm:ss")}. JobId: {document.JobId}.";

            logs.Add(new JobDataLogDocument
            {
                JobId = document.JobId,
                JobName = document.JobName,
                Level = "Information",
                Description = description,
                CreatedDate = document.StartDate
            });

            return Task.CompletedTask;
        }

        public Task Finish(JobDataEndDocument document)
        {
            var history = histories.FirstOrDefault((x) => x.JobId == document.JobId);

            if (history != null)
            {
                history.EndDate = document.EndDate;
                history.ElapsedMilliseconds = document.ElapsedMilliseconds;
            }

            var description = $"{document.JobName} finished executing in {document.EndDate.ToString("dddd, dd MMMM yyyy HH:mm:ss")}. JobId: {document.JobId}. Total Elapsed Milliseconds: {document.ElapsedMilliseconds}.";

            logs.Add(new JobDataLogDocument
            {
                JobId = document.JobId,
                JobName = document.JobName,
                Level = "Information",
                Description = description,
                CreatedDate = DateTime.Now
            });

            return Task.CompletedTask;
        }

        public Task Log(JobDataLogDocument document)
        {
            logs.Add(document);

            return Task.CompletedTask;
        }

        public Task<JobDataHistoryDocument> GetLastHistoryByName(string jobName)
        {
            var history = histories.OrderByDescending((x) => x.StartDate).FirstOrDefault((x) => x.JobName == jobName);

            return Task.FromResult(history);
        }

        public Task<List<JobDataHistoryDocument>> GetHistoriesByName(string jobName)
        {
            var historyList = histories.Where((x) => x.JobName == jobName).ToList();

            return Task.FromResult(historyList);
        }

        public Task<JobDataStatisticDocument> GetStatisticByName(string jobName)
        {
            var historyList = histories.Where((x) => x.JobName == jobName && x.StartDate > DateTime.Now.AddDays(-10)).ToList();

            var groups = historyList
                    .Select((x) => new
                    {
                        x.StartDate.Date,
                        x.ElapsedMilliseconds
                    })
                    .GroupBy((x) => x.Date)
                    .OrderBy((x) => x.Key);

            var result = new JobDataStatisticDocument
            {
                Labels = groups.Select((x) => x.Key.ToString("dd.MM")).ToArray(),
                Series = groups.Select((x) => x.Average((y) => y.ElapsedMilliseconds)).ToArray()
            };

            return Task.FromResult(result);
        }

        public Task<List<JobDataLogDocument>> GetLogsByJobId(string jobId)
        {
            var logList = logs.Where((x) => x.JobId == jobId).OrderByDescending((x) => x.CreatedDate).ToList();

            return Task.FromResult(logList);
        }
    }
}
