using static TUNING.BUILDINGS;
using static TUNING.MATERIALS;
using static AdvancedGeneratos.Common.GeneratorCommonConstants;

namespace AdvancedGeneratos.Generators
{
    public class ThermoelectricGenerator
    {
        public const string ID = nameof(ThermoelectricGenerator);

        public const string ANISTR = "generatormerc_kanim";

        public const int WATT = 250;

        public const int Heat_Self = -8;
        public const int Heat_Exhaust = -120;

        public const float MinimumTemp = 283.15f;

        public static readonly LogicPorts.Port[] INPUT_PORTS = GetPorts(new CellOffset(1, 0));

        public static readonly LocString NAME = Fal("Thermoelectric Generator", ID);
        public static readonly LocString DESC = $"Removes {Fal("Heat", "HEAT")} from environment {-Heat_Self - Heat_Exhaust} kDTUs per second.";
        public const string EFFECT = "Removes heat and produces electricity.";

        public static readonly string[] Materials = new[] { REFINED_METAL };
        public static readonly float[] MateMassKg = new[] { MASS_KG.TIER4 };

        public static readonly string ID_UPPER = ID.ToUpper();
    }
}
