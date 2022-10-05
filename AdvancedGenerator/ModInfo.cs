using System.Reflection;
using MissileLib;

namespace AdvancedGenerators
{
    public class ModInfo: IModInfo
    {
        public string Name => "Advanced Generators";
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}