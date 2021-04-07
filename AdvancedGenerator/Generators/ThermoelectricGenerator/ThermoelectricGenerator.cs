using AdvancedGenerators.Common;
using TUNING;
using UnityEngine;
using static AdvancedGenerators.Common.GeneratorCommonConstants;

namespace AdvancedGenerators.Generators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ThermoelectricGenerator : IBuildingConfig
    {
        public const string Id = nameof(ThermoelectricGenerator);
        private const int Width = 4;
        private const int Height = 3;

        private const string AnimationString = "generatormerc_kanim";

        private const int HitPoints = GeneratorCommonConstants.HitPoints;
        private const float ConstructionTime = GeneratorCommonConstants.ConstructionTime;
        private readonly float[] _mateMassKg = new[] {BUILDINGS.MASS_KG.TIER4};
        private readonly string[] _materials = new[] {MATERIALS.REFINED_METAL};

        private readonly EffectorValues _decorRating = DECOR.BONUS.TIER0;
        private readonly EffectorValues _noisePollutionRating = NOISE_POLLUTION.NOISY.TIER6;

        private const int Watt = 256;

        private const int HeatSelf = -10;
        private const int HeatExhaust = -118;

        private const float MinimumTemp = 283.15f;
        private const float MeltingPoint = BUILDINGS.MELTING_POINT_KELVIN.TIER1;

        public static readonly LogicPorts.Port[] InputPorts = GetPorts(new CellOffset(1, 0));

        public static readonly LocString Name = Fal("Thermoelectric Generator", Id);

        public static readonly LocString Description =
            $"Converts {Fal("Heat", "HEAT")} from environment to electrical {Fal("Power", "POWER")}.\n" +
            $"{-HeatSelf - HeatExhaust} kDTUs per second.";

        public const string Effect = "Converts Heat and produces electricity.";

        public static string SliderTooltipKey = "STRINGS.UI.UISIDESCREENS.WIRELESS_AUTOMATION_SIDE_SCREEN.TOOLTIP";
        public static string SliderTooltip = "Amount of heat that transfer from the environment to the generator.";

        public static string SliderTitleKey = "STRINGS.UI.UISIDESCREENS.WIRELESS_AUTOMATION_SIDE_SCREEN.TITLE";
        public static string SliderTitle = "Cooling aggressiveness";

        public static readonly string IdUpper = Id.ToUpper();

        public override BuildingDef CreateBuildingDef()
        {
            var bd = BuildingTemplates.CreateBuildingDef(Id, Width, Height, AnimationString, HitPoints, ConstructionTime,
                _mateMassKg, _materials, MeltingPoint, BuildLocationRule.OnFloor, _decorRating, _noisePollutionRating);

            bd.GeneratorWattageRating = bd.GeneratorBaseCapacity = Watt;
            bd.SelfHeatKilowattsWhenActive = HeatSelf;
            bd.ExhaustKilowattsWhenActive = HeatExhaust;
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

        private static void RegisterPorts(GameObject go) => GeneratedBuildings.RegisterSingleLogicInputPort(go);
    }
}