using System.Collections.Generic;
using System.Threading;
using SnapshotService.CommandHandler.ListVolumesHandler;
using SnapshotService.CommandHandler.SnapshotsHandler;
using SnapshotService.CommandHandler.TagsHandler;
using SnapshotService.CommandHandler.VirtualMachinesHandler;
using SnapshotService.Dto;

namespace SnapshotService.Helper
{
    public static class CommandHelper
    {
        public static ListVirtualMachinesResponse GetVirtualMachines()
        {
            var cmd = new ListVirtualMachinesCommand();
            cmd.ExecuteCommand();
            Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
            return cmd.ListVirtualMachinesResponse;
        }

        public static ListVolumesResponse GetVolumes(string virtualMachineId)
        {
            var cmd = new ListVolumesCommand(virtualMachineId);
            cmd.ExecuteCommand();
            Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
            return cmd.ListVolumesResponse;
        }

        public static ListSnapshotsResponse GetSnapshots(string volumeId)
        {
            var cmd = new ListSnapshotCommand(volumeId);
            cmd.ExecuteCommand();
            Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
            return cmd.ListSnapshotsResponse;
        }

        public static QueryAsyncJobResultResponse CreateSnapshots(string volumeId)
        {
            var cmd = new CreateSnapshotCommand(volumeId);
            cmd.ExecuteCommand();
            Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
            return cmd.QueryAsyncJobResultResponse;
        }

        public static QueryAsyncJobResultResponse DeleteSnapshots(string snapshotId)
        {
            var cmd = new DeleteSnapshotCommand(snapshotId);
            cmd.ExecuteCommand();
            Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
            return cmd.QueryAsyncJobResultResponse;
        }

        public static QueryAsyncJobResultResponse CreateTagsForSnapshot(string resourceId)
        {
            var resourceIds = new List<string> { resourceId };
            var tags = new Dictionary<string, string> { {ConfigurationHelper.TagKey, ConfigurationHelper.TagValue} };

            var cmd = new CreateTagsCommand(resourceIds, tags, CreateTagsCommand.SnapshotResourceType);
            cmd.ExecuteCommand();
            Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
            return cmd.QueryAsyncJobResultResponse;
        }
    }
}
