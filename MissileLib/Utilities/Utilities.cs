using System.IO;
using System.Reflection;

namespace MissileLib.Utilities
{
    public static class Utilities
    {
        public static string RunFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    }
}