using static TUNING.BUILDINGS;
using static TUNING.MATERIALS;
using static AdvancedGenerators.Common.GeneratorCommonConstants;

namespace AdvancedGenerators.Generators
{
    public class RefinedCarbonGenerator
    {
        public const string ID = nameof(RefinedCarbonGenerator);
        public const string AnimSTR = "generatorphos_kanim";

        public static readonly LocString NAME = Fal("refined carbon generator", ID);
        public static readonly LocString DESC = $"{Fal(" Coal ", " COAL ")} Produces more electricity than a generator.";
        public const string EFFECT = "Burns refined carbon and produces a lot of electricity.";

        public const int WATT = 1200;
        public const float CARBONE_BURN_RATE = 1f;
        public const float CARBONE_CAPACITY = 500f;
        public const float REFILL_CAPACITY = 100f;

        public const float CO2_GEN_RATE = 0.02f;
        public const float OUT_CO2_TEMP = 348.15f;

        public static readonly string[] Materials = new[] { METAL, BUILDABLERAW };
        public static readonly float[] MateMassKg = new[] { MASS_KG.TIER5, MASS_KG.TIER4 };

        public static readonly string ID_UPPER = ID.ToUpper();
    }
}
