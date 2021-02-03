using System;
using System.Threading;
using Newtonsoft.Json;
using SnapshotService.CommandHandler.JobHandler;
using SnapshotService.Dto;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler.SnapshotsHandler
{
    public class CreateSnapshotCommand : CommandBase
    {
        private const string VolumeKey = "volumeid";

        public CreateSnapshotResponse CreateSnapshotResponse { get; set; }
        public QueryAsyncJobResultResponse QueryAsyncJobResultResponse { get; set; }

        public CreateSnapshotCommand(string volumeId)
        {
            CommandDictionary.Add(CommandKey, "createSnapshot");
            CommandDictionary.Add(VolumeKey, volumeId);
            CommandDictionary.Add(ApiKey, ConfigurationHelper.GetApiKey());
        }

        public override string ExecuteCommand()
        {
            try
            {
                var contentResult = base.ExecuteCommand();
                var result = JsonConvert.DeserializeObject<CreateSnapshotResponseDto>(contentResult);
                CreateSnapshotResponse = result.CreateSnapshotResponse;

                if (string.IsNullOrEmpty(CreateSnapshotResponse.JobId) == false)
                {
                    Thread.Sleep(ConfigurationHelper.SnapshotCreatedDelayInSeconds);

                    var queryAsyncJobResults = new QueryAsyncJobResultCommand(CreateSnapshotResponse.JobId);
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
