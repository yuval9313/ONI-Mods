using AdvancedGenerators.Common;
using AdvancedGenerators.Generators;
using TUNING;
using UnityEngine;
using static AdvancedGenerators.Common.GeneratorCommonConstants;

namespace AdvancedGenerators.GeneratorsBuildConfigs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EcoFriendlyMethaneGenerator : IBuildingConfig
    {
        public const string Id = nameof(EcoFriendlyMethaneGenerator);
        public static readonly LocString Name = Fal("Green Gas Generator", Id);

        public static readonly LocString Description =
            "Converts Natural Gas and Oxygen into electrical Power. Emits exhaust and water.";

        public static readonly string Effect =
            $"Burns {Fal("Natural Gas", "METHANE")} to create {Fal("Power", "POWER")}, " +
            $"using {Fal("Oxygen", "OXYGEN")} and {Fal("Sand", "SAND")}.\r\n" +
            $"Produces {Fal("Carbon Dioxide", "CARBONDIOXIDE")} and {Fal("Water", "WATER")}";
        
        private const string AnimationString = "generatormethane_kanim";
        
        private const int HitPoints = GeneratorCommonConstants.HitPoints;
        private const float ConstructTime = GeneratorCommonConstants.ConstructionTime;
        private const float MeltingPoint = GeneratorCommonConstants.MeltingPoint;

        private const int HeatSelf = 4;
        private const int HeatExhaust = 2;

        private const int FilterMaxStored = 400;

        private const float UseMethane = 0.1f;
        public const float MaxStored = 60;
        private const float ExhaustH2O = 0.07f;
        private const float OxygenMaxStored = 1f;
        private const float OxygenConsumptionRate = 0.1f;
        private const float ExhaustCo2 = 0.03f;

        private static readonly Tag Filter = new Tag("Filter");

        private static readonly string[] Materials = new[] {MATERIALS.METAL, MATERIALS.REFINED_METAL};
        private static readonly float[] MateMassKg = new[] {BUILDINGS.MASS_KG.TIER5, BUILDINGS.MASS_KG.TIER1};

        private const int Watt = 1000;

        public static readonly LogicPorts.Port[] Input = GetPorts(new CellOffset(0, 0));

        public static readonly string IdUpper = Id.ToUpper();
        
        public override BuildingDef CreateBuildingDef()
        {
            var bd = BuildingTemplates.CreateBuildingDef(Id, 4, 3, AnimationString, HitPoints, ConstructTime,
                MateMassKg, Materials, MeltingPoint, BuildLocationRule.OnFloor, DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER6);

            bd.GeneratorBaseCapacity = bd.GeneratorWattageRating = Watt;
            bd.ExhaustKilowattsWhenActive = HeatExhaust;
            bd.SelfHeatKilowattsWhenActive = HeatSelf;

            bd.ViewMode = OverlayModes.Power.ID;
            bd.AudioCategory = MetalAudio;

            bd.PowerOutputOffset = bd.UtilityInputOffset = new CellOffset(0, 0);
            bd.UtilityOutputOffset = new CellOffset(2, 2);
            bd.RequiresPowerOutput = true;

            bd.PermittedRotations = PermittedRotations.FlipH;

            bd.InputConduitType = bd.OutputConduitType = ConduitType.Gas;

            return bd;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go) => RegisterPorts(go);

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => RegisterPorts(go);

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<LoopingSounds>();

            Storage ist = go.AddOrGet<Storage>(), ost = go.AddComponent<Storage>();

            var cc = go.AddOrGet<ConduitConsumer>();
            cc.storage = ist;
            cc.conduitType = ConduitType.Gas;
            cc.consumptionRate = 0.2f;
            cc.capacityKG = 1;
            cc.capacityTag = SimHashes.Methane.CreateTag();
            cc.forceAlwaysSatisfied = true;
            cc.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

            var cd = go.AddOrGet<ConduitDispenser>();
            cd.conduitType = ConduitType.Gas;
            cd.storage = ost;

            var ec = go.AddOrGet<ElementConsumer>();
            ec.storage = ist;
            ec.configuration = ElementConsumer.Configuration.Element;
            ec.elementToConsume = SimHashes.Oxygen;
            ec.capacityKG = OxygenMaxStored;
            ec.consumptionRate = OxygenConsumptionRate;
            ec.consumptionRadius = 2;
            ec.isRequired = ec.storeOnConsume = true;

            var manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(ist);
            manualDeliveryKg.allowPause = false;
            manualDeliveryKg.capacity = FilterMaxStored;
            manualDeliveryKg.refillMass = 10;
            manualDeliveryKg.requestedItemTag = Filter;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.GeneratePower.Id;
            
            var adg = go.AddOrGet<GasPoweredGenerator>();
            adg.Consumer = ec;
            adg.InOutItems = new EnergyGenerator.Formula
            {
                inputs = new[]
                {
                    new EnergyGenerator.InputItem(cc.capacityTag, UseMethane, 1),
                    new EnergyGenerator.InputItem(SimHashes.Oxygen.CreateTag(), ExhaustCo2, OxygenMaxStored),
                    new EnergyGenerator.InputItem(Filter, ExhaustH2O, 50)
                },
                outputs = new[]
                {
                    new EnergyGenerator.OutputItem(SimHashes.Water, ExhaustH2O, false, new CellOffset(1, 1), 348.15f),
                    new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, ExhaustCo2, true, 383.15f)
                }
            };
            adg.InStorage = ist;
            adg.OutStorage = ost;

            Tinkerable.MakeFarmTinkerable(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            RegisterPorts(go);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}
