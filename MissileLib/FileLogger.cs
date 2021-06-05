using System.IO;

namespace MissileLib
{
    public class FileLogger : ILogger
    {
        private IModInfo _modInfo;
        private readonly string _playTime = System.DateTime.UtcNow.ToString("yy-MM-dd");

        public string LogsDir { get; set; }
        
        public void InitLogger(IModInfo modInfo)
        {
            _modInfo = modInfo;
            LogsDir = Path.Combine(Utilities.Utilities.RunFolder, "Logs");
            if (!Directory.Exists(LogsDir)) Directory.CreateDirectory(LogsDir);
        }

        public void Log(string message)
        {
            var logName = $"{_playTime}-{_modInfo.Name}.log";
            var logPath = Path.Combine(LogsDir, logName);
            if (!File.Exists(logPath)) File.Create(logPath);
            using (var w = File.AppendText(logPath))
            {
                w.WriteLine(message);
            }
        }
    }
}