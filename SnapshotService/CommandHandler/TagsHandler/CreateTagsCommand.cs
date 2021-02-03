using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using SnapshotService.CommandHandler.JobHandler;
using SnapshotService.Dto;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler.TagsHandler
{
    public class CreateTagsCommand : CommandBase
    {
        private const string ResourceIdsKey = "resourceids";
        private const string ResourceTypeKey = "resourcetype";
        private const string TagsKey = "tags[0].key";
        private const string TagsValue = "tags[0].value";

        public const string SnapshotResourceType = "snapshot";
        public const string VirtualMachineResourceType = "UserVm";

        public CreateTagsResponse CreateTagsResponse { get; set; }
        public QueryAsyncJobResultResponse QueryAsyncJobResultResponse { get; set; }

        public CreateTagsCommand(List<string> resourceIds, Dictionary<string, string> tags, string resourceType)
        {
            // Currently support one tag at a time for one resource
            CommandDictionary.Add(CommandKey, "createTags");
            CommandDictionary.Add(ResourceIdsKey, resourceIds.FirstOrDefault());
            CommandDictionary.Add(ResourceTypeKey, resourceType);
            CommandDictionary.Add(TagsKey, tags.Keys.FirstOrDefault());
            CommandDictionary.Add(TagsValue, tags.Values.FirstOrDefault());
            CommandDictionary.Add(ApiKey, ConfigurationHelper.GetApiKey());
        }

        public override string ExecuteCommand()
        {
            try
            {
                var contentResult = base.ExecuteCommand();
                var result = JsonConvert.DeserializeObject<CreateTagsResponseDto>(contentResult);
                CreateTagsResponse = result.CreateTagsResponse;

                if (string.IsNullOrEmpty(CreateTagsResponse.JobId) == false)
                {
                    Thread.Sleep(ConfigurationHelper.CreateTagsDelayInSeconds);

                    var queryAsyncJobResults = new QueryAsyncJobResultCommand(CreateTagsResponse.JobId);
                    var jobContentResult = queryAsyncJobResults.ExecuteCommand();

                    if (string.IsNullOrEmpty(jobContentResult) == false)
                    {
                        QueryAsyncJobResultResponse = queryAsyncJobResults.QueryAsyncJobResultResponse;
                    }
                }
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
