using System.Collections.Generic;

namespace SnapshotService.Dto
{
    public class ListSnapshotsResponseDto
    {
        public ListSnapshotsResponse ListSnapshotsResponse { get; set; }
    }

    public class ListSnapshotsResponse
    {
        public int Count { get; set; }
        public List<Snapshot> Snapshot { get; set; }
    }
}
