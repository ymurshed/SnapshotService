using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapshotService.Helper
{
    public class ConfigurationHelper
    {
        // For key remove last

        public static string GetComputeEndpoint()
        {
            var endPoint = "https://api.exoscale.com/compute";
            return endPoint;
        }

        public static string GetApiKey()
        {
            var apiKey = "EXO4b0a25bd29b58db8c47a46e44";
            return apiKey;
        }

        public static string GetSecretKey()
        {
            var secretKey = "vAz9CJdl9KhKlIXVxQLjb0PKdiRgW6hlr58rNEPlxPQQ";
            return secretKey;
        }

        public static List<string> GetExcludedServerNames()
        {
            var splitChars = new[] {","};
            var serverNames =  "Test1,Test2";
            var splitedServerNames = serverNames.Split(splitChars, StringSplitOptions.None).ToList();
            return splitedServerNames;
        }

        public static List<string> GetIncludedServerNames()
        {
            var splitChars = new[] { "," };
            var serverNames = "Test3,Test4";
            var splitedServerNames = serverNames.Split(splitChars, StringSplitOptions.None).ToList();
            return splitedServerNames;
        }

        private static string GetTagKey()
        {
            var tagKey = "snapshot-tag-key";
            return tagKey;
        }

        private static string GetTagValue()
        {
            var tagValue = "created-by-api";
            return tagValue;
        }

        public static string GetLogPath()
        {
            var logPath = "C:\\SnapshotLog";
            return logPath;
        }

        private static int GetSnapshotDeleteAfterHours()
        {
            var hours = "12";
            var deleteAfterHours = int.Parse(hours); 
            return deleteAfterHours;
        }

        public static int MaxHourDifference => GetSnapshotDeleteAfterHours();

        public static int ReadyToRunAfterInMins => 20 * 60 * 1000;
        public static int SnapshotCreatedDelayInSeconds => 35 * 1000;
        public static int SnapshotDeletedDelayInSeconds => 35 * 1000;
        public static int CreateTagsDelayInSeconds => 35 * 1000;
        public static int CommandExecutionDelayInSeconds => 5 * 1000;

        public static string TagKey => GetTagKey();
        public static string TagValue => GetTagValue();
    }
}
