using System;
using System.Threading;
using Newtonsoft.Json;
using SnapshotService.CommandHandler.JobHandler;
using SnapshotService.Dto;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler.SnapshotsHandler
{
    public class DeleteSnapshotCommand : CommandBase
    {
        private const string SnapshotIdKey = "id";
        
        public DeleteSnapshotResponse DeleteSnapshotResponse { get; set; }
        public QueryAsyncJobResultResponse QueryAsyncJobResultResponse { get; set; }

        public DeleteSnapshotCommand(string snapshotId)
        {
            CommandDictionary.Add(CommandKey, "deleteSnapshot");
            CommandDictionary.Add(SnapshotIdKey, snapshotId);
            CommandDictionary.Add(ApiKey, ConfigurationHelper.GetApiKey());
        }

        public override string ExecuteCommand()
        {
            try
            {
                var contentResult = base.ExecuteCommand();
                var result = JsonConvert.DeserializeObject<DeleteSnapshotResponseDto>(contentResult);
                DeleteSnapshotResponse = result.DeleteSnapshotResponse;

                if (string.IsNullOrEmpty(DeleteSnapshotResponse.JobId) == false)
                {
                    Thread.Sleep(ConfigurationHelper.SnapshotDeletedDelayInSeconds);

                    var queryAsyncJobResults = new QueryAsyncJobResultCommand(DeleteSnapshotResponse.JobId);
                    var jobContentResult = queryAsyncJobResults.ExecuteCommand();

                    if (string.IsNullOrEmpty(jobContentResult) == false)
                    {
                        QueryAsyncJobResultResponse = queryAsyncJobResults.QueryAsyncJobResultResponse;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteAllLog(e.Message, LogHelper.LogType.Error);
                throw;
            }
            return string.Empty;
        }
    }
}
