using System;
using System.Collections.Generic;

namespace SnapshotService.Dto
{
    public class Snapshot
    {
        public string Id { get; set; }
        public string SnapshotType { get; set; }
        public long Size { get; set; }
        public string VolumeId { get; set; } // Require to create a snapshot
        public string VolumeName { get; set; }
        public string VolumeType { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string IntervalType { get; set; }
        public string State { get; set; }
        public List<Tags> Tags { get; set; }
        public bool Revertable { get; set; }
    }
}