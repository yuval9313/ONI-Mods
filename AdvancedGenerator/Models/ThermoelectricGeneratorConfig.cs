using AdvancedGenerators.Common;
using TUNING;

namespace AdvancedGenerators.Models
{
    public struct ThermoelectricGeneratorConfig
    {
        public LocString Description;
        
        public int Watt;
        
        public int HeatSelf;
        public int HeatExhaust;
        public float MinimumTemp;
        
        public string SliderTooltip;
        public string SliderTitle;

        public const int Width = 4;
        public const int Height = 3;

        public const string AnimationString = "generatormerc_kanim";
        
        public static readonly LogicPorts.Port[] InputPorts = GeneratorCommonConstants.GetPorts(new CellOffset(1, 0));

        public const float MeltingPoint = BUILDINGS.MELTING_POINT_KELVIN.TIER1;
        public static readonly float[] MaterialMassKg = new[] {BUILDINGS.MASS_KG.TIER4};
        public static readonly string[] Materials = new[] {MATERIALS.REFINED_METAL};

        public static EffectorValues DecorRating = DECOR.BONUS.TIER0;
        public static EffectorValues NoisePollutionRating = NOISE_POLLUTION.NOISY.TIER6;

        public const string SliderTooltipKey = "STRINGS.UI.UISIDESCREENS.WIRELESS_AUTOMATION_SIDE_SCREEN.TOOLTIP";
        public static string SliderTitleKey = "STRINGS.UI.UISIDESCREENS.WIRELESS_AUTOMATION_SIDE_SCREEN.TITLE";
    }
}