using System;
using Newtonsoft.Json;
using SnapshotService.Dto;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler.VirtualMachinesHandler
{
    public class ListVirtualMachinesCommand : CommandBase
    {
        public ListVirtualMachinesResponse ListVirtualMachinesResponse { get; set; }

        public ListVirtualMachinesCommand()
        {
            CommandDictionary.Add(CommandKey, "listVirtualMachines");
            CommandDictionary.Add(ApiKey, ConfigurationHelper.GetApiKey());
        }

        public override string ExecuteCommand()
        {
            try
            {
                var contentResult = base.ExecuteCommand();
                var result = JsonConvert.DeserializeObject<ListVirtualMachinesResponseDto>(contentResult);
                ListVirtualMachinesResponse = result.ListVirtualMachinesResponse;
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
