using UnityEngine;
using static AdvancedGeneratos.Common.GeneratorCommonConstants;
using TUNING;

namespace AdvancedGeneratos
{
    public class ThermoelectricGenerator : IBuildingConfig
    {
        public const string ID = nameof(ThermoelectricGenerator);
        const int Width = 4;
        const int Height = 3;

        const string AnimationString = "generatormerc_kanim";

        const int HitPoints = 100;
        const int ConstructionTime = 45;
        readonly float[] MateMassKg = new[] { BUILDINGS.MASS_KG.TIER4 };
        readonly string[] Materials = new[] { MATERIALS.REFINED_METAL };

        readonly EffectorValues DecorRating = DECOR.BONUS.TIER0;
        readonly EffectorValues NoisePollutionRating = NOISE_POLLUTION.NOISY.TIER6;

        public const int WATT = 250;

        public const int Heat_Self = -10;
        public const int Heat_Exhaust = -118;

        public const float MinimumTemp = 283.15f;
        public const float MeltingPoint = BUILDINGS.MELTING_POINT_KELVIN.TIER3;

        public static readonly LogicPorts.Port[] INPUT_PORTS = GetPorts(new CellOffset(1, 0));

        public static readonly LocString NAME = Fal("Thermoelectric Generator", ID);
        public static readonly LocString DESC = $"Converts {Fal("Heat", "HEAT")} from environment to electrical {Fal("Power", "POWER")}.\n{-Heat_Self - Heat_Exhaust} kDTUs per second.";
        public const string EFFECT = "Converts Heat and produces electricity.";


        public static readonly string ID_UPPER = ID.ToUpper();

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef bd = BuildingTemplates.CreateBuildingDef(ID, Width, Height, AnimationString, HitPoints, ConstructTime,
                MateMassKg, Materials, MeltingPoint, BuildLocationRule.OnFloor, DecorRating, NoisePollutionRating, 0.1f);

            bd.GeneratorWattageRating = bd.GeneratorBaseCapacity = WATT;
            bd.ExhaustKilowattsWhenActive = Heat_Exhaust;
            bd.SelfHeatKilowattsWhenActive = Heat_Self;
            bd.ViewMode = OverlayModes.Temperature.ID;
            bd.ModifiesTemperature = true;

            bd.AudioCategory = AU_METAL;
            bd.PowerOutputOffset = new CellOffset(1, 0);

            return bd;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go) => RegisterPorts(go);

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => RegisterPorts(go);

        public override void DoPostConfigureComplete(GameObject go)
        {
            RegisterPorts(go);
            go.AddOrGet<LogicOperationalController>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

            go.AddOrGet<LoopingSounds>();

            go.AddOrGet<ThermoelectricPowerGenerator>();
            go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = MinimumTemp;

            Tinkerable.MakePowerTinkerable(go);
            go.AddOrGetDef<PoweredActiveController.Def>();
        }

        protected void RegisterPorts(GameObject go) =>
            GeneratedBuildings.RegisterSingleLogicInputPort(go);
    }
}
