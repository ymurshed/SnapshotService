using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using SnapshotService.Helper;

namespace SnapshotService.CommandHandler
{
    public abstract class CommandBase
    {
        public const string CommandKey = "command";
        public const string ApiKey = "apikey";
        public const string Signature = "signature";

        public Dictionary<string, object> CommandDictionary;

        protected CommandBase()
        {
            CommandDictionary = new Dictionary<string, object>();
        }

        private string CastValue(object value)
        {
            var newValue = value is List<string> ? ((List<string>)value).FirstOrDefault() : value.ToString();
            return newValue;
        }

        private string GetQueryParam()
        {
            var queryParam = CommandDictionary.OrderBy(k => k.Key).Aggregate("", (current, item) =>
                current + "&" + item.Key + "=" + CastValue(item.Value));
            return queryParam.Remove(0, 1).ToLower();
        }

        private static byte[] GetUtfEncodedBytes(string input)
        {
            var utf8 = new UTF8Encoding();
            var encodedBytes = utf8.GetBytes(input);
            return encodedBytes;
        }

        private static string ComputeSha(byte[] key, byte[] commandInput)
        {
            var myhmacsha1 = new HMACSHA1(key);
            var stream = new MemoryStream(commandInput);
            var sha = myhmacsha1.ComputeHash(stream);
            var signature = Convert.ToBase64String(sha);
            return signature;
        }

        private string GetSignature()
        {
            var queryParam = GetQueryParam();
            var secretKey = ConfigurationHelper.GetSecretKey();

            var commandInput = GetUtfEncodedBytes(queryParam);
            var key = GetUtfEncodedBytes(secretKey);

            var signature = ComputeSha(key, commandInput);
            return signature;
        }

        private string GetCommandExecutionUrl()
        {
            var signature = HttpUtility.UrlEncode(GetSignature());
            CommandDictionary.Add(Signature, signature);

            var param = string.Join("&", CommandDictionary.Select(kvp =>
                        $"{kvp.Key}={CastValue(kvp.Value)}"));

            var url = $"{ConfigurationHelper.GetComputeEndpoint()}?{param}";
            return url;
        }

        public virtual string ExecuteCommand()
        {
            try
            {
                var executionCommandUrl = GetCommandExecutionUrl();
                //LogHelper.WriteAllLog($"Command execution url: {executionCommandUrl}", LogHelper.LogType.Info);

                var result = HttpClientHelper.GetResult(executionCommandUrl);
                var contentResult = result.Content.ReadAsStringAsync().Result;
                return contentResult;
            }
            catch (Exception e)
            {
                LogHelper.WriteAllLog(e.Message, LogHelper.LogType.Error);
                throw;
            }
        }
    }
}
