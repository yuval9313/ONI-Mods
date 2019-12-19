namespace AdvancedGeneratos
{
    using static STRINGS.UI;
    using static TUNING.BUILDINGS;
    using static TUNING.MATERIALS;
    internal class Constans
    {
        public const string Kpath = "STRINGS.BUILDINGS.PREFABS.";
        public const int HITPT = HITPOINTS.TIER1;
        public const float ConstructTime = CONSTRUCTION_TIME_SECONDS.TIER3;
        public const float MeltingPoint = MELTING_POINT_KELVIN.TIER2;
        public const string TabCategory = "Power";

        public const string AU_METAL = "Metal";
        public const string AU_HOLLOWMETAL = "HollowMetal";

        public static LogicPorts.Port[] GetPorts(CellOffset offset) =>
            new[] { LogicPorts.Port.InputPort(
                LogicOperationalController.PORT_ID,
                offset,
                LOGIC_PORTS.CONTROL_OPERATIONAL,
                LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE,
                LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE
                )};


        public static readonly LogicPorts.Port[] INPUT_PORT_00 = GetPorts(new CellOffset(0, 0));

        public static string Fal(string text, string id) => FormatAsLink(text, id);

        public class RefinedCarbonGenerator
        {
            public const string ID = nameof(RefinedCarbonGenerator);
            public const string AnimSTR = "generatorphos_kanim";

            public static readonly LocString NAME = Fal("refined carbon generator", ID);
            public static readonly LocString DESC = $"{Fal (" Coal "," COAL ")} Produces more electricity than a generator.";
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
            public static readonly LocString DESC = $"Removes {Fal("Heat", "HEAT")} from environment {-Heat_Self-Heat_Exhaust} kDTUs per second.";
            public const string EFFECT = "Removes heat and produces electricity.";

            public static readonly string[] Materials = new[] { REFINED_METAL };
            public static readonly float[] MateMassKg = new[] { MASS_KG.TIER4 };

            public static readonly string ID_UPPER = ID.ToUpper();
        }

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
            public static readonly LocString DESC = $"{Fal (" Nafta "," NAPHTHA ")} and {Fal (" Oxygen "," OXYGEN ")} and electricity using {Fal (" Carbon Dioxide "," CARBONDIOXIDE ")} To produce. ";
             public const string EFFC = "Need oxygen and fuel naphtha.";

            public static readonly string[] Materials = new[] { REFINED_METAL, PLASTIC };
            public static readonly float[] MateMassKg = new[] { MASS_KG.TIER3, MASS_KG.TIER3 };

            public const int Watt = 850;

            public static readonly string ID_UPPER = ID.ToUpper();
        }

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
            public static readonly LocString DESC = $"{Fal (" Natural Gas "," METHANE ")} and {Fal (" Oxygen "," OXYGEN ")}, using electricity and {Fal (" carbon dioxide "," CARBONDIOXIDE ")}, and {Fal (" Water "," WATER ")}.";
             public const string EFFECT = "Requires oxygen and filtration media, and does not drain contaminated water.";

            public static readonly string[] TMate = new[] { METAL, REFINED_METAL };
            public static readonly float[] TMass = new[] { MASS_KG.TIER5, MASS_KG.TIER1 };

            public static readonly Tag Filter = new Tag("Filter");

            public const int Watt = 1000;

            public static readonly LogicPorts.Port[] INPUT = GetPorts(new CellOffset(0, 0));

            public static readonly string ID_UPPER = ID.ToUpper();
        }
    }
}
