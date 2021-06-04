using System;
using System.Linq;
using System.Reflection;
// ReSharper disable StaticMemberInGenericType

namespace MissileLib
{
    public static class MissileLogger
    {
        private const string NoticeFormat = "<!----- {0} -----!>";

        private static readonly string LibDescription =
            $"MissileLib: {Assembly.GetExecutingAssembly().GetName().Version}";

        private static IModInfo _modInfo;
        public static ILogger Logger { get; private set; }
        
        private static string Notice(string message) => string.Format(NoticeFormat, message);

        public static void InitLogger<T>() where T: ILogger, new()
        {
            Logger = new T();
            _modInfo = DiscoverModInfo();
            Logger.InitLogger(_modInfo ?? new PartialModInfo(Assembly.GetExecutingAssembly().GetName().Name));
            LogLoggerInitializationStatus();
        }
        
        private static IModInfo DiscoverModInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var modInfo = assembly.GetExportedTypes().FirstOrDefault(p => p.GetInterfaces().Contains(typeof(IModInfo)));
            if (modInfo == null) return null;
            var modInfoInstance = (IModInfo) Activator.CreateInstance(modInfo);
            return modInfoInstance;
        }
        
        private static void LogLoggerInitializationStatus()
        {
            if (_modInfo == null)
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                Log(Notice($"{LibDescription} Logger couldn't find version of MOD: {assemblyName}"));
            }
            else
            {
                var modDescription = $"{_modInfo.Name}: {_modInfo.Version}";
                Log(Notice($"{LibDescription} Init for {modDescription}"));
            }
        }

        public static void Log(string message)
        {
            Logger.Log($"{Timestamp()} - {message}");
        }
        
        public static void Log(object obj)
        {
            Log(obj.ToString());
        }
        
        private static string Timestamp() => System.DateTime.UtcNow.ToString("[HH:mm:ss.fff]");
    }
}