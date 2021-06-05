using AdvancedGenerators.Common;
using AdvancedGenerators.Models;
using UnityEngine;
using static AdvancedGenerators.Common.GeneratorCommonConstants;

namespace AdvancedGenerators.Generators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ThermoelectricGenerator : IBuildingConfig, IAdvancedGeneratorConfig
    {
#pragma warning disable 649
        private ThermoelectricGeneratorConfig _config;

        public static string Id = nameof(ThermoelectricGenerator);
        public LocString Name => Fal("Thermoelectric Generator", Id);
        public string Description => "Converts Heat and produces electricity.";
        public string Effect =>
            $"Converts {Fal("Heat", "HEAT")} from environment to electrical {Fal("Power", "POWER")}.";
        
        public static readonly string IdUpper = Id.ToUpper();
        
#pragma warning restore 649
        
        public override BuildingDef CreateBuildingDef()
        {
            var bd = BuildingTemplates.CreateBuildingDef(Id, 
                ThermoelectricGeneratorConfig.Width, ThermoelectricGeneratorConfig.Height,
                ThermoelectricGeneratorConfig.AnimationString, 
                HitPoints, ConstructionTime, ThermoelectricGeneratorConfig.MaterialMassKg,
                ThermoelectricGeneratorConfig.Materials, ThermoelectricGeneratorConfig.MeltingPoint,
                BuildLocationRule.OnFloor, ThermoelectricGeneratorConfig.DecorRating,
                ThermoelectricGeneratorConfig.NoisePollutionRating);

            bd.GeneratorWattageRating = bd.GeneratorBaseCapacity = _config.Watt;
            bd.SelfHeatKilowattsWhenActive = _config.HeatSelf;
            bd.ExhaustKilowattsWhenActive = _config.HeatExhaust;
            bd.ViewMode = OverlayModes.Temperature.ID;
            bd.ModifiesTemperature = true;

            bd.AudioCategory = MetalAudio;
            bd.PowerOutputOffset = new CellOffset(1, 0);
            
            Debug.Log($"Config value is: {_config.Watt}");
            
            return bd;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go) => RegisterPorts(go);

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => RegisterPorts(go);

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratorCommonConstants.RegisterPorts(go);
            go.AddOrGet<LogicOperationalController>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

            go.AddOrGet<LoopingSounds>();

            go.AddOrGet<ThermoelectricPowerGenerator>();
            go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = _config.MinimumTemp;

            Tinkerable.MakePowerTinkerable(go);
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}