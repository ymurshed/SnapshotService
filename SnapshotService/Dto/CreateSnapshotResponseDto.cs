namespace SnapshotService.Dto
{
    public class CreateSnapshotResponseDto
    {
        public CreateSnapshotResponse CreateSnapshotResponse { get; set; }
    }

    public class CreateSnapshotResponse
    {
        public string Id { get; set; }
        public string JobId { get; set; }
    }
}
