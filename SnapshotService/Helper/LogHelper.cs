using System;
using System.IO;

namespace SnapshotService.Helper
{
    public class LogHelper
    {
        public static string LogPath = "";

        public enum LogType { None, Error, Warning, Info, Success }

        public static void WriteAllLog(string message, LogType logType)
        {
            var type = logType == LogType.None ? "" : "\n[" + logType + "]\n----------------------\n";
            var fullMessage = type + message;

            var today = DateTime.Now;
            var fileName = $"{today.Day}-{today.Month}-{today.Year}.txt";
            var path = Path.Combine(LogPath, fileName);

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (var sw = File.CreateText(path))
                {
                    sw.WriteLine(fullMessage);
                }
                return;
            }

            // This text is always added, making the file longer over time if it is not deleted.
            using (var sw = File.AppendText(path))
            {
                sw.WriteLine(fullMessage);
            }
        }
    }
}
