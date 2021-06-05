using static EventSystem;

namespace AdvancedGenerators.Generators
{
    public class ThermoelectricPowerGenerator : Generator
    {
        // [field: Serialize]
        // public int CoolingAggressiveness { get; set; } = -ThermoelectricGenerator.HeatExhaust - ThermoelectricGenerator.HeatSelf;
    
        private HandleVector<int>.Handle _accumulator = HandleVector<int>.Handle.InvalidHandle;
        protected const int OnActivateChangeFlag = 824508782;
        protected static readonly IntraObjectHandler<ThermoelectricPowerGenerator> OnActivateChangeDelegate = new IntraObjectHandler<ThermoelectricPowerGenerator>(OnActivateChangedStatic);

        public bool HasMeter = true;
        public Meter.Offset MeterOffset = Meter.Offset.Infront;
        protected MeterController meter;

        private static void OnActivateChangedStatic(ThermoelectricPowerGenerator gen, object data) =>
            gen.OnActivateChanged(data as Operational);

        protected override void OnSpawn()
        {
            base.OnSpawn();
            _accumulator = Game.Instance.accumulators.Add("Power", (KMonoBehaviour) this);
            Subscribe(OnActivateChangeFlag, OnActivateChangeDelegate);
            CreateMeter();
        }
        
        private void CreateMeter() => this.meter = new MeterController((KAnimControllerBase) this.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
        {
            "meter_OL",
            "meter_frame",
            "meter_fill"
        });

        protected virtual void OnActivateChanged(Operational op) =>
            selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, op.IsActive ?
                Db.Get().BuildingStatusItems.Wattage :
                Db.Get().BuildingStatusItems.GeneratorOffline, this);

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);

            operational.SetFlag(wireConnectedFlag, CircuitID != ushort.MaxValue);
            operational.SetActive(operational.IsOperational);

            if (HasMeter)
                MeterSet();

            if (!operational.IsOperational) return;
            
            GenerateJoules(WattageRating * dt, true);
            selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
        }

        protected virtual void MeterSet() => meter.SetPositionPercent(operational.IsActive ? 1 : 0);


        // private void ChangeCoolingAggressiveness(int agressiveness)
        // {
        //     CoolingAggressiveness = agressiveness;
        //     var selfExhaust = Convert.ToSingle(agressiveness / 11.8);
        //     var thermoGenBd = building.Def;
        //     Debug.Log(WattageRating);
        //     thermoGenBd.ExhaustKilowattsWhenActive = -selfExhaust;
        //     thermoGenBd.SelfHeatKilowattsWhenActive = -(agressiveness - selfExhaust);
        //     thermoGenBd.GeneratorWattageRating = Convert.ToSingle(CoolingAggressiveness * 2);
        // }

        // public int SliderDecimalPlaces(int index) => 0;
        // public float GetSliderMin(int index) => 80;
        // public float GetSliderMax(int index) => 130;
        // public float GetSliderValue(int index) => CoolingAggressiveness;
        // public void SetSliderValue(float value, int index) => ChangeCoolingAggressiveness(Mathf.RoundToInt(value));
        // public string GetSliderTooltipKey(int index) => ThermoelectricGenerator.SliderTooltipKey;
        // public string GetSliderTooltip() => $"Will cool down {UI.FormatAsKeyWord($"{CoolingAggressiveness} kDTUs")} per second on {UI.FormatAsKeyWord($"{WattageRating} Watt")}";
        // public string SliderTitleKey => ThermoelectricGenerator.SliderTitleKey + ThermoelectricGenerator.SliderTitle;
        // public string SliderUnits => string.Empty;
    }
}
