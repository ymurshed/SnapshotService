using System;
using Newtonsoft.Json;
using SnapshotService.Dto;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler.SnapshotsHandler
{
    public class ListSnapshotCommand : CommandBase
    {
        private const string VolumeKey = "volumeid";

        public ListSnapshotsResponse ListSnapshotsResponse { get; set; }

        public ListSnapshotCommand(string volumeId)
        {
            CommandDictionary.Add(CommandKey, "listSnapshots");
            CommandDictionary.Add(VolumeKey, volumeId);
            CommandDictionary.Add(ApiKey, ConfigurationHelper.GetApiKey());
        }

        public override string ExecuteCommand()
        {
            try
            {
                var contentResult = base.ExecuteCommand();
                var result = JsonConvert.DeserializeObject<ListSnapshotsResponseDto>(contentResult);
                ListSnapshotsResponse = result.ListSnapshotsResponse;
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
