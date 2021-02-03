using System;
using System.Collections.Generic;

namespace SnapshotService.Dto
{
    public class ListVolumesResponseDto
    {
        public ListVolumesResponse ListVolumesResponse { get; set; }
    }

    public class ListVolumesResponse
    {
        public int Count { get; set; }
        public List<Volume> Volume { get; set; }
    }

    public class Volume
    {
        public string Id { get; set; } // Volume Id
        public int DeviceId { get; set; }
        public string VirtualMachineId { get; set; }
        public string VmName { get; set; }
        public string VmDisplayName { get; set; }
        public string VmState { get; set; }
        public DateTime Created { get; set; }
        public string State { get; set; }
    }
}
