using System;

namespace SnapshotService.Dto
{
    public class QueryAsyncJobResultResponseDto
    {
        public QueryAsyncJobResultResponse QueryAsyncJobResultResponse { get; set; }
    }

    public class QueryAsyncJobResultResponse
    {
        public string UserId { get; set; }
        public string Cmd { get; set; }
        public int JobStatus { get; set; }
        public int JobProcStatus { get; set; }
        public int JobResultCode { get; set; }
        public object JobResulTtype { get; set; }
        public JobResult JobResult { get; set; }
        public DateTime Created { get; set; }
        public string JobId { get; set; }
    }

    public class JobResult
    {
        public Snapshot Snapshot { get; set; }
    }
}
