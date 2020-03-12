using static TUNING.BUILDINGS;
using static TUNING.MATERIALS;
using static AdvancedGeneratos.Common.GeneratorCommonConstants;

namespace AdvancedGeneratos.Generators
{
    public class EcoFriendlyMethaneGenerator
    {
        public const string ID = nameof(EcoFriendlyMethaneGenerator);

        public const string ANISTR = "generatormethane_kanim";

        public const int Heat_Self = 4;
        public const int Heat_Exhaust = 2;

        public const int FilterMaxStored = 400;

        public const float UseMethane = 0.1f;
        public const float MaxStored = 60;
        public const float ExhaustH2O = 0.07f;
        public const float Oxygen_MaxStored = 1f;
        public const float OxygenCosumRate = 0.1f;
        public const float ExhaustCO2 = 0.03f;

        public static readonly LocString NAME = Fal("Green Gas Generator", ID);
        public static readonly LocString DESC = $"{Fal(" Natural Gas ", " METHANE ")} and {Fal(" Oxygen ", " OXYGEN ")}, using electricity and {Fal(" carbon dioxide ", " CARBONDIOXIDE ")}, and {Fal(" Water ", " WATER ")}.";
        public const string EFFECT = "Requires oxygen and filtration media, and does not drain contaminated water.";

        public static readonly string[] TMate = new[] { METAL, REFINED_METAL };
        public static readonly float[] TMass = new[] { MASS_KG.TIER5, MASS_KG.TIER1 };

        public static readonly Tag Filter = new Tag("Filter");

        public const int Watt = 1000;

        public static readonly LogicPorts.Port[] INPUT = GetPorts(new CellOffset(0, 0));

        public static readonly string ID_UPPER = ID.ToUpper();
    }
}
