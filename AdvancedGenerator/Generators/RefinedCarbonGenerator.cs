using AdvancedGenerators.Common;
using Epic.OnlineServices.Stats;
using TUNING;
using UnityEngine;
using static AdvancedGenerators.Common.GeneratorCommonConstants;

namespace AdvancedGenerators.Generators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class RefinedCarbonGenerator : IBuildingConfig
    {
        public const string Id = nameof(RefinedCarbonGenerator);
        private const string AnimationString = "generatorphos_kanim";

        public static readonly LocString Name = Fal("Refined Carbon Generator", Id);

        public static readonly LocString Description =
            "Produce much more electricity than coal generator, by burning refined carbon, emits exhaust";

        public static readonly string Effect =
            $"Burns {Fal("Refined Carbon", "REFINEDCARBON")} into {Fal("Power", "POWER")}.";

        private const int HitPoints = GeneratorCommonConstants.HitPoints;
        private const float ConstructTime = GeneratorCommonConstants.ConstructionTime;
        private const float MeltingPoint = GeneratorCommonConstants.MeltingPoint;
        
        private const int Watt = 1200;
        private const float CarbonBurnRate = 1f;
        private const float CarbonCapacity = 500f;
        private const float RefillCapacity = 100f;

        private const float Co2GenerationRate = 0.02f;
        private const float OutCo2Temperature = 348.15f;

        private static readonly string[] Materials = new[] { MATERIALS.METAL, MATERIALS.BUILDABLERAW };
        private static readonly float[] MateMassKg = new[] { BUILDINGS.MASS_KG.TIER5, BUILDINGS.MASS_KG.TIER4 };

        public static readonly string IdUpper = Id.ToUpper();
        
        public override BuildingDef CreateBuildingDef()
        {
            var bd = BuildingTemplates.CreateBuildingDef(Id, 3, 3, AnimationString, HitPoints, ConstructTime,
                MateMassKg, Materials, MeltingPoint, BuildLocationRule.OnFloor, DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER5);

            bd.GeneratorWattageRating = bd.GeneratorBaseCapacity = Watt;
            bd.ExhaustKilowattsWhenActive = 0f;
            bd.SelfHeatKilowattsWhenActive = 4f;
            bd.ViewMode = OverlayModes.Power.ID;

            bd.AudioCategory = AU_HOLLOWMETAL;
            bd.AudioSize = "large";

            return bd;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

            var t = GameTagExtensions.Create(SimHashes.RefinedCarbon);

            var eg = go.AddOrGet<EnergyGenerator>();
            eg.formula = new EnergyGenerator.Formula
            {
                inputs = new EnergyGenerator.InputItem[]
                {
                    new EnergyGenerator.InputItem(t, CarbonBurnRate, CarbonCapacity)
                },
                outputs = new EnergyGenerator.OutputItem[]
                {
                    new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, Co2GenerationRate, false, new CellOffset(1, 0), OutCo2Temperature)
                }
            };
            eg.powerDistributionOrder = 8;

            var st = go.AddOrGet<Storage>();
            st.capacityKg = CarbonCapacity;
            st.showInUI = true;

            go.AddOrGet<LoopingSounds>();
            Prioritizable.AddRef(go);

            var manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.allowPause = false;
            manualDeliveryKg.SetStorage(st);
            manualDeliveryKg.requestedItemTag = t;
            manualDeliveryKg.capacity = st.capacityKg;
            manualDeliveryKg.refillMass = RefillCapacity;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

            Tinkerable.MakePowerTinkerable(go);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => RegisterPorts(go);

        public override void DoPostConfigureUnderConstruction(GameObject go) => RegisterPorts(go);

        public override void DoPostConfigureComplete(GameObject go)
        {
            RegisterPorts(go);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }

        private static void RegisterPorts(GameObject go) =>
            GeneratedBuildings.RegisterSingleLogicInputPort(go);
    }
}
