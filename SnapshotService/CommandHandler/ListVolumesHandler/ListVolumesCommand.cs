using System;
using Newtonsoft.Json;
using SnapshotService.Dto;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler.ListVolumesHandler
{
    public class ListVolumesCommand : CommandBase
    {
        private const string VirtualMachineIdKey = "virtualmachineid";

        public ListVolumesResponse ListVolumesResponse { get; set; }

        public ListVolumesCommand(string virtualMachineId)
        {
            CommandDictionary.Add(CommandKey, "listVolumes");
            CommandDictionary.Add(VirtualMachineIdKey, virtualMachineId);
            CommandDictionary.Add(ApiKey, ConfigurationHelper.GetApiKey());
        }

        public override string ExecuteCommand()
        {
            try
            {
                var contentResult = base.ExecuteCommand();
                var result = JsonConvert.DeserializeObject<ListVolumesResponseDto>(contentResult);
                ListVolumesResponse = result.ListVolumesResponse;
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
