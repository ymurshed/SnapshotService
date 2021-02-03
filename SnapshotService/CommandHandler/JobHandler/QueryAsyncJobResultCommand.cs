using System;
using Newtonsoft.Json;
using SnapshotService.Dto;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler.JobHandler
{
    public class QueryAsyncJobResultCommand : CommandBase
    {
        private const string JobIdKey = "jobid";

        public QueryAsyncJobResultResponse QueryAsyncJobResultResponse { get; set; }

        public QueryAsyncJobResultCommand(string jobId)
        {
            CommandDictionary.Add(CommandKey, "queryAsyncJobResult");
            CommandDictionary.Add(JobIdKey, jobId);
            CommandDictionary.Add(ApiKey, ConfigurationHelper.GetApiKey());
        }

        public override string ExecuteCommand()
        {
            var contentResult = string.Empty;
            try
            {
                contentResult = base.ExecuteCommand();
                var result = JsonConvert.DeserializeObject<QueryAsyncJobResultResponseDto>(contentResult);
                QueryAsyncJobResultResponse = result.QueryAsyncJobResultResponse;
            }
            catch (Exception e)
            {
                LogHelper.WriteAllLog(e.Message, LogHelper.LogType.Error);
                throw;
            }
            return contentResult;
        }
    }
}
