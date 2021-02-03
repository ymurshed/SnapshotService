using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SnapshotService.Helper;

namespace SnapshotService.SnapshotWorkflow
{
    public class SnapshotWorkflow
    {
        // Key: Virtual Machine Name, Value: Volume Ids of that Virtual Machine
        public static Dictionary<string, List<string>> MapVirtualMachineVolume;
        public static Dictionary<string, List<Dto.Snapshot>> MapVolumeSnapshots;

        private static void Init()
        {
            MapVirtualMachineVolume = new Dictionary<string, List<string>>();
            MapVolumeSnapshots = new Dictionary<string, List<Dto.Snapshot>>();
        }

        public void ExecuteWorkflow()
        {
            var msg = $"----->>> Start executing workflow at: {DateTime.Now}";
            LogHelper.WriteAllLog(msg, LogHelper.LogType.Info);

            Init();

            // Step 1: Find all Virtual Machines (servers) and map with their Volume
            msg = "Executing step 1: Map virtual machine with volume";
            LogHelper.WriteAllLog(msg, LogHelper.LogType.Info);

            MapVirtualMachineWithVolumes();


            // Step 2: Map all Snapshots with their Volume
            msg = "Executing step 2: Map volume with snapshots";
            LogHelper.WriteAllLog(msg, LogHelper.LogType.Info);

            if (MapVirtualMachineVolume.Any())
            {
                foreach (var key in MapVirtualMachineVolume.Keys)
                {
                    // Assuming one volume per virtual machine
                    var volumeId = MapVirtualMachineVolume[key].FirstOrDefault();
                    MapVolumeWithSnapshots(volumeId);
                }
            }


            // Step 3: Snapshot creation and deletion steps
            msg = "Executing step 3: Create snapshots and delete older snapshots";
            LogHelper.WriteAllLog(msg, LogHelper.LogType.Info);

            if (MapVolumeSnapshots.Any())
            {
                foreach (var key in MapVolumeSnapshots.Keys)
                {
                    var snapShots = MapVolumeSnapshots[key];

                    // This volume has no snapshot created yet. Therefore instantly create a new snapshot
                    if (snapShots == null)
                    {
                        TrySnapshotCreate(key);
                    }
                    // For the existing snapshot 
                    else
                    {
                        var lastCreatedSnapshot = snapShots.OrderByDescending(o => o.Created).FirstOrDefault();
                        var createNewSnapshot = lastCreatedSnapshot != null &&
                                                (DateTime.Now - lastCreatedSnapshot.Created).Hours >
                                                ConfigurationHelper.MaxHourDifference;

                        // Create new snapshot and remove older snapshots. Deleting snapshot have some condition.
                        if (createNewSnapshot)
                        {
                            var result = TrySnapshotCreate(key);

                            // If snapshot created, then remove older snapshots based on condition. Otherwise do nothing.
                            if (result)
                            {
                                // Deleted snapshots based on Tag, those are created thru api
                                // Filter first level based on tag key
                                var snapshotsToDelete = snapShots.FindAll(o => o.Tags.Select(x => x.Key).ToList()
                                                        .Contains(ConfigurationHelper.TagKey));

                                // Filter second level based on tag value
                                snapshotsToDelete = snapshotsToDelete.FindAll(o => o.Tags.Select(x => x.Value).ToList()
                                                    .Contains(ConfigurationHelper.TagValue));

                                foreach (var snapshot in snapshotsToDelete)
                                {
                                    TrySnapshotDelete(snapshot.Id, key);
                                }
                            }
                        }
                    }

                    LogHelper.WriteAllLog("\n", LogHelper.LogType.None);
                }
            }


            // Step 4: Log summary
            msg = "Executing step 4: Logging summary of the current running snapshots";
            LogHelper.WriteAllLog(msg, LogHelper.LogType.Info);

            LogSummary();

            msg = $"----->>> End of execution workflow at: {DateTime.Now}";
            LogHelper.WriteAllLog(msg, LogHelper.LogType.Info);
        }

        private static bool TrySnapshotCreate(string volumeId)
        {
            var jobResultResponse = CommandHelper.CreateSnapshots(volumeId);
            if (jobResultResponse.JobStatus <= 0)
            {
                // If snapshot stauts is still showing not backed-up, then wait for few seconds and check again
                Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
                var listSnapshots = CommandHelper.GetSnapshots(volumeId);

                if (listSnapshots.Count <= 0)
                {
                    var msg = "Snapshot creation failed. " +
                              $"Server = '{GetServerByVolume(volumeId)}', " +
                              $"Volume '{volumeId}'";

                    LogHelper.WriteAllLog(msg, LogHelper.LogType.Warning);
                    return false;
                }
            }

            var msg1 = $"Snapshot = '{jobResultResponse.JobResult.Snapshot.Id}' created successfully. " +
                       $"Server = '{GetServerByVolume(volumeId)}', " +
                       $"Volume = '{volumeId}'";

            LogHelper.WriteAllLog(msg1, LogHelper.LogType.Success);

            // Create tag for the current snapshot
            TryTagCreate(jobResultResponse.JobResult.Snapshot.Id, volumeId);
            return true;
        }

        private static void TrySnapshotDelete(string snapshotId, string volumeId)
        {
            var jobResultResponse = CommandHelper.DeleteSnapshots(snapshotId);
            if (jobResultResponse.JobStatus <= 0)
            {
                // If snapshot stauts is still showing not deleted, then wait for few seconds and check again
                Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);
                var listSnapshots = CommandHelper.GetSnapshots(volumeId);

                if (listSnapshots.Count > 0 && listSnapshots.Snapshot.Any(o => o.Id == snapshotId))
                {
                    var msg = "Snapshot deletion failed. " +
                              $"Server = '{GetServerByVolume(volumeId)}', " +
                              $"Volume '{volumeId}'";

                    LogHelper.WriteAllLog(msg, LogHelper.LogType.Warning);
                    return;
                }
            }

            var msg1 = $"Snapshot = '{snapshotId}' deleted successfully. " +
                       $"Server = '{GetServerByVolume(volumeId)}', " +
                       $"Volume = '{volumeId}'";

            LogHelper.WriteAllLog(msg1, LogHelper.LogType.Success);
        }

        private static void TryTagCreate(string snapshotId, string volumeId)
        {
            var jobResultResponse = CommandHelper.CreateTagsForSnapshot(snapshotId);

            // If status is less than 0, then wait for sometime to get the actual status
            if (jobResultResponse.JobStatus <= 0)
                Thread.Sleep(ConfigurationHelper.CommandExecutionDelayInSeconds);

            var listSnapshots = CommandHelper.GetSnapshots(volumeId);
            if (listSnapshots.Count > 0 && listSnapshots.Snapshot.Any(o => o.Id == snapshotId))
            {
                var snapshot = listSnapshots.Snapshot.Find(o => o.Id == snapshotId);

                if (snapshot.Tags.FirstOrDefault()?.Key == ConfigurationHelper.TagKey &&
                    snapshot.Tags.FirstOrDefault()?.Value == ConfigurationHelper.TagValue)
                {
                    var msg = "Snapshot tag created successfully. " +
                              $"Server = '{GetServerByVolume(volumeId)}', " +
                              $"Volume '{volumeId}'";

                    LogHelper.WriteAllLog(msg, LogHelper.LogType.Success);
                    return;
                }

                var msg1 = "Snapshot tag creation failed. " +
                           $"Server = '{GetServerByVolume(volumeId)}', " +
                           $"Volume '{volumeId}'";

                LogHelper.WriteAllLog(msg1, LogHelper.LogType.Warning);
            }
        }

        private static void MapVirtualMachineWithVolumes()
        {
            var listVirtualMachines = CommandHelper.GetVirtualMachines();
            var excludedServerNames = ConfigurationHelper.GetExcludedServerNames();
            var includedServerNames = ConfigurationHelper.GetIncludedServerNames();

            if (listVirtualMachines == null || listVirtualMachines.Count == 0)
                return;

            // Excludes servers based on config
            var shortListedVirtualMachines = listVirtualMachines.VirtualMachine
                .FindAll(o => excludedServerNames.Contains(o.DisplayName.Trim()) == false);

            // Includes servers only based on config
            shortListedVirtualMachines = shortListedVirtualMachines
                .FindAll(o => includedServerNames.Contains(o.DisplayName.Trim()));

            foreach (var virtualMachine in shortListedVirtualMachines)
            {
                var listVolumes = CommandHelper.GetVolumes(virtualMachine.Id);
                if (listVolumes.Count > 0)
                {
                    var volumeIds = listVolumes.Volume.Select(o => o.Id).ToList();
                    MapVirtualMachineVolume.Add(virtualMachine.DisplayName, volumeIds);
                }
            }
        }

        private static void MapVolumeWithSnapshots(string volumeId)
        {
            var listSnapshots = CommandHelper.GetSnapshots(volumeId);
            MapVolumeSnapshots.Add(volumeId, listSnapshots.Count > 0 ? listSnapshots.Snapshot : null);
        }

        private static string GetServerByVolume(string volumneId)
        {
            return MapVirtualMachineVolume.FirstOrDefault(o => o.Value.Contains(volumneId)).Key;
        }

        private static void LogSummary()
        {
            var fullLog = "";

            foreach (var item in MapVirtualMachineVolume)
            {
                var volumeId = item.Value.FirstOrDefault();
                var listSnapshots = CommandHelper.GetSnapshots(volumeId);

                if (listSnapshots.Count > 0)
                {
                    var msg = $"Server = '{GetServerByVolume(volumeId)}' " +
                              $"Volume = '{volumeId}' following snapshots are running ===>>>\n";

                    foreach (var snapshot in listSnapshots.Snapshot.OrderByDescending(o => o.Created))
                    {
                        msg += $"SnapshotId = '{snapshot.Id}', Created = '{snapshot.Created}', State = '{snapshot.State}'\n";
                    }

                    fullLog += msg + "\n";
                }
            }

            if (!string.IsNullOrEmpty(fullLog))
            {
                LogHelper.WriteAllLog(fullLog, LogHelper.LogType.Info);
            }
        }
    }
}
