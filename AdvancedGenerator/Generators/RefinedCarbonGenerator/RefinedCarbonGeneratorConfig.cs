﻿using TUNING;
using UnityEngine;
using static AdvancedGeneratos.Common.GeneratorCommonConstants;
using static AdvancedGeneratos.Generators.RefinedCarbonGenerator;

namespace AdvancedGeneratos.Generators.Config
{
    class RefinedCarbonGeneratorConfig : IBuildingConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef bd = BuildingTemplates.CreateBuildingDef(ID, 3, 3, AnimSTR, HITPT, ConstructTime,
                MateMassKg, Materials, MeltingPoint, BuildLocationRule.OnFloor, DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER5);

            bd.GeneratorWattageRating = bd.GeneratorBaseCapacity = WATT;
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

            Tag t = GameTagExtensions.Create(SimHashes.RefinedCarbon);

            EnergyGenerator eg = go.AddOrGet<EnergyGenerator>();
            eg.formula = new EnergyGenerator.Formula
            {
                inputs = new EnergyGenerator.InputItem[]
                {
                    new EnergyGenerator.InputItem(t, CARBONE_BURN_RATE, CARBONE_CAPACITY)
                },
                outputs = new EnergyGenerator.OutputItem[]
                {
                    new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, CO2_GEN_RATE, false, new CellOffset(1, 0), OUT_CO2_TEMP)
                }
            };
            eg.powerDistributionOrder = 8;

            Storage st = go.AddOrGet<Storage>();
            st.capacityKg = CARBONE_CAPACITY;
            st.showInUI = true;

            go.AddOrGet<LoopingSounds>();
            Prioritizable.AddRef(go);

            ManualDeliveryKG mdkg = go.AddOrGet<ManualDeliveryKG>();
            mdkg.allowPause = false;
            mdkg.SetStorage(st);
            mdkg.requestedItemTag = t;
            mdkg.capacity = st.capacityKg;
            mdkg.refillMass = REFILL_CAPACITY;
            // mdkg.choreTags = new Tag[] { GameTags.ChoreTypes.Power };
            mdkg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

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

        private void RegisterPorts(GameObject go) =>
            GeneratedBuildings.RegisterSingleLogicInputPort(go);
    }
}
