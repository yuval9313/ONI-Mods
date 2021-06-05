using AdvancedGenerators.Common;
using TUNING;
using UnityEngine;
using static AdvancedGenerators.Common.GeneratorCommonConstants;

namespace AdvancedGenerators.Generators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NaphthaGenerator : IBuildingConfig
    {
        public const string Id = nameof(NaphthaGenerator);

        private const string AnimationString = "generatorpetrol_kanim";

        private const int HeatSelf = 8;
        private const int HeatExhaust = 1;

        private const float UseNaphtha = 1f;
        private const float NaphthaMaxStored = 10;
        private const float OxygenMaxStored = 1f;
        private const float OxygenConsumptionRate = 0.1f;
        private const float ExhaustCo2 = 0.04f;

        private const int Watt = 850;
        
        public static readonly LocString Name = Fal("Naphtha Generator", Id);

        public static readonly LocString Description =
            "Produce more power than coal, Using naphtha and oxygen, but emits heat and exhaust.";
        public static readonly string Effect = 
            $"Uses {Fal("Naphtha", "NAPHTHA")} and {Fal("Oxygen", "OXYGEN")} to produce " + 
            $"{Fal("Power", "POWER")} and exhaust {Fal("Carbon Dioxide", "CARBONDIOXIDE")}.";

        private static readonly string[] Materials = new[] { MATERIALS.REFINED_METAL, MATERIALS.PLASTIC };
        private static readonly float[] MateMassKg = new[] { BUILDINGS.MASS_KG.TIER3, BUILDINGS.MASS_KG.TIER3 };


        public static readonly string IdUpper = Id.ToUpper();
        
        public override BuildingDef CreateBuildingDef()
        {
            var bd = BuildingTemplates.CreateBuildingDef(Id, 3, 4, AnimationString, 100, 100,
                MateMassKg, Materials, MeltingPoint, BuildLocationRule.OnFloor, DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER4);

            bd.GeneratorWattageRating = bd.GeneratorBaseCapacity = Watt;
            bd.ExhaustKilowattsWhenActive = HeatExhaust;
            bd.SelfHeatKilowattsWhenActive = HeatSelf;

            bd.ViewMode = OverlayModes.Power.ID;

            bd.AudioCategory = MetalAudio;

            bd.UtilityInputOffset = new CellOffset(-1, 0);
            bd.PowerOutputOffset = new CellOffset(0, 0);

            bd.RequiresPowerOutput = true;

            bd.InputConduitType = ConduitType.Liquid;

            return bd;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => RegisterPorts(go);

        public override void DoPostConfigureUnderConstruction(GameObject go) => RegisterPorts(go);

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<LoopingSounds>();

            var st = go.AddOrGet<Storage>();
            st.showInUI = true;

            var cc = go.AddOrGet<ConduitConsumer>();
            cc.conduitType = ConduitType.Liquid;
            cc.consumptionRate = 10;
            cc.capacityKG = 100;
            cc.forceAlwaysSatisfied = true;
            cc.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

            var ec = go.AddOrGet<ElementConsumer>();
            ec.storage = st;
            ec.configuration = ElementConsumer.Configuration.Element;
            ec.elementToConsume = SimHashes.Oxygen;
            ec.capacityKG = OxygenMaxStored;
            ec.consumptionRate = OxygenConsumptionRate;
            ec.consumptionRadius = 2;
            ec.isRequired = ec.storeOnConsume = true;

            var aeg = go.AddOrGet<GasPoweredGenerator>();
            aeg.InStorage = st;
            aeg.OutStorage = go.AddComponent<Storage>();
            aeg.Consumer = ec;
            aeg.InOutItems = new EnergyGenerator.Formula
            {
                inputs = new EnergyGenerator.InputItem[]
                {
                    new EnergyGenerator.InputItem(SimHashes.Naphtha.CreateTag(), UseNaphtha, NaphthaMaxStored),
                    new EnergyGenerator.InputItem(SimHashes.Oxygen.CreateTag(), ExhaustCo2, OxygenMaxStored)
                },
                outputs = new EnergyGenerator.OutputItem[]
                {
                    new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, ExhaustCo2, false, new CellOffset(0, 0), 320.15f)
                }
            };

            Tinkerable.MakePowerTinkerable(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            RegisterPorts(go);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }

        protected void RegisterPorts(GameObject go) =>
            GeneratedBuildings.RegisterSingleLogicInputPort(go);
    }
}
