using static TUNING.BUILDINGS;
using static TUNING.MATERIALS;
using static AdvancedGeneratos.Common.CommonConstans;

namespace AdvancedGeneratos.Generators
{
    public class NaphthaGenerator
    {
        public const string ID = nameof(NaphthaGenerator);

        public const string ANISTR = "generatorpetrol_kanim";

        public const int Heat_Self = 8;
        public const int Heat_Exhaust = 1;

        public const float UseNaphtha = 1f;
        public const float Naphtha_MaxStored = 10;
        public const float Oxygen_MaxStored = 1f;
        public const float OxygenCosumRate = 0.1f;
        public const float ExhaustCO2 = 0.04f;

        public static readonly LocString NAME = Fal("naphtha generator", ID);
        public static readonly LocString DESC = $"{Fal(" Nafta ", " NAPHTHA ")} and {Fal(" Oxygen ", " OXYGEN ")} and electricity using {Fal(" Carbon Dioxide ", " CARBONDIOXIDE ")} To produce. ";
        public const string EFFC = "Need oxygen and fuel naphtha.";

        public static readonly string[] Materials = new[] { REFINED_METAL, PLASTIC };
        public static readonly float[] MateMassKg = new[] { MASS_KG.TIER3, MASS_KG.TIER3 };

        public const int Watt = 850;

        public static readonly string ID_UPPER = ID.ToUpper();
    }
}
