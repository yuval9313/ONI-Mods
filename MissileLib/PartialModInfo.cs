namespace MissileLib
{
    public class PartialModInfo: IModInfo
    {
        public string Name { get; }
        public string Version => "Missing version";

        public PartialModInfo()
        {
            
        }
        public PartialModInfo(string name)
        {
            Name = name;
        }
    }
}