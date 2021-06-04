using System;
using System.IO;

namespace MissileLib
{
    public class FileLogger : ILogger
    {
        public string LogsDir { get; set; }
        private IModInfo _modInfo;

        public void InitLogger(IModInfo modInfo)
        {
            _modInfo = modInfo;
            LogsDir = Path.Combine(Utilities.Utilities.RunFolder, "Logs");
            if (!Directory.Exists(LogsDir)) Directory.CreateDirectory(LogsDir);
        }

        public void Log(string message)
        {
            Console.WriteLine($"Mid - logging : {_modInfo.Name}");
            var logName = System.DateTime.UtcNow.ToString("yy-MM-dd") + $" {_modInfo.Name}.log";
            Console.WriteLine($"Log name - {logName}");
            var logPath = Path.Combine(LogsDir, logName);
            Console.WriteLine($"Log path - {logPath}");
            if (!File.Exists(logPath)) File.Create(logPath);
            using (var w = File.AppendText(logPath))
            {
                w.WriteLine(message);
            }
        }
    }
}