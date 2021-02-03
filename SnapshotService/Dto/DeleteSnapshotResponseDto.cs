namespace SnapshotService.Dto
{
    public class DeleteSnapshotResponseDto
    {
        public DeleteSnapshotResponse DeleteSnapshotResponse { get; set; }
    }

    public class DeleteSnapshotResponse
    {
        public string JobId { get; set; }
    }
}
