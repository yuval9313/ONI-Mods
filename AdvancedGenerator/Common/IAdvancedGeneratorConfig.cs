namespace AdvancedGenerators.Common
{
    public interface IAdvancedGeneratorConfig
    {
        LocString Name { get; }
        string Description { get; }
        string Effect { get; } 
    }
}