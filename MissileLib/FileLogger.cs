using System;
using System.IO;

namespace MissileLib
{
    public class FileLogger : ILogger
    {
        private IModInfo _modInfo;
        private readonly string _playTime = System.DateTime.UtcNow.ToString("yy-MM-dd");
        private string _logPath;

        public string LogsDir { get; set; }
        
        public void InitLogger(IModInfo modInfo)
        {
            _modInfo = modInfo;
            LogsDir = Path.Combine(Utilities.Utilities.RunFolder, "Logs");
            if (!Directory.Exists(LogsDir)) Directory.CreateDirectory(LogsDir);
            var logName = $"{_playTime}-{_modInfo.Name}.log";
            _logPath = Path.Combine(LogsDir, logName);
        }

        public void Log(string message)
        {
            using (var w = File.AppendText(_logPath))
            {
                w.WriteLine(message);
            }
        }
    }
}