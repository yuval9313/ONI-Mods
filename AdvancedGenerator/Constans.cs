namespace AdvancedGeneratos
{
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
    }
}
