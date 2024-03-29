﻿using AdvancedGenerators.Common;


namespace AdvancedGenerators.Generators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class GasPoweredGenerator : AdvancedEnergyGenerator
    {
        [MyCmpGet]
        public ElementConsumer Consumer;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Consumer.EnableConsumption(false);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Consumer.EnableConsumption(true);
        }

        protected override bool LogicOnCheckPre(bool isOn)
        {
            Consumer.EnableConsumption(isOn);
            return isOn;
        }
    }
}
