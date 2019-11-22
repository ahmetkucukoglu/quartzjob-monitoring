namespace QuartzJobMonitoring
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IJobDataSource
    {
        Task Start(JobDataStartDocument document);
        Task Finish(JobDataEndDocument document);
        Task Log(JobDataLogDocument document);
        Task<JobDataHistoryDocument> GetLastHistoryByName(string jobName);
        Task<List<JobDataHistoryDocument>> GetHistoriesByName(string jobName);
        Task<JobDataStatisticDocument> GetStatisticByName(string jobName);
        Task<List<JobDataLogDocument>> GetLogsByJobId(string jobId);
    }
}
