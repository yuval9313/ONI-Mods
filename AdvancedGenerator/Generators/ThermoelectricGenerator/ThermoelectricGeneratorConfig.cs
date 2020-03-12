using TUNING;
using UnityEngine;
using static AdvancedGeneratos.Common.GeneratorCommonConstants;
using static AdvancedGeneratos.Generators.ThermoelectricGenerator;

namespace AdvancedGeneratos
{
    public class ThermoelectricGeneratorConfig : IBuildingConfig
    {
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef bd = BuildingTemplates.CreateBuildingDef(ID, 4, 3, ANISTR, 100, 45,
                MateMassKg, Materials, MeltingPoint, BuildLocationRule.OnFloor, DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER6);

            bd.GeneratorWattageRating = bd.GeneratorBaseCapacity = WATT;
            bd.ExhaustKilowattsWhenActive = Heat_Exhaust;
            bd.SelfHeatKilowattsWhenActive = Heat_Self;
            bd.ViewMode = OverlayModes.Temperature.ID;
            bd.ModifiesTemperature = true;

            Debug.Log($"\nTempurature for thermoGen: {bd.ModifiesTemperature}!!!\n");

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
