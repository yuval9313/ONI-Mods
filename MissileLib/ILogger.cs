
namespace MissileLib
{
    public interface ILogger
    {
        void InitLogger(IModInfo modInfo);
        void Log(string message);
    }
}