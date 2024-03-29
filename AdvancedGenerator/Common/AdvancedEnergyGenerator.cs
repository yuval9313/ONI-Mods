using System;
using System.Collections.Generic;
using UnityEngine;
using static EnergyGenerator;
using static EventSystem;

namespace AdvancedGenerators.Common
{
    public class AdvancedEnergyGenerator : Generator
    {
        protected const int OnActivateChangeFlag = 824508782;
        protected static readonly IntraObjectHandler<AdvancedEnergyGenerator> OnActivateChangeDelegate = new IntraObjectHandler<AdvancedEnergyGenerator>(OnActivateChangedStatic);

        public bool HasMeter = true;
        public Storage InStorage;
        public Storage OutStorage;
        public Meter.Offset MeterOffset;
        public Formula InOutItems = new Formula();
        protected MeterController meter;

        private static void OnActivateChangedStatic(AdvancedEnergyGenerator gen, object data) => gen.OnActivateChanged(data as Operational);

        protected virtual void OnActivateChanged(Operational data) =>
            selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, data.IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline, this);

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(OnActivateChangeFlag, OnActivateChangeDelegate);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            OnSpawnPost();
        }

        protected virtual void OnSpawnPost()
        {
            if (InStorage == null || OutStorage == null || InOutItems.inputs == null || InOutItems.outputs == null)
                throw new Exception("Storage or input, output not specified.");

            if (HasMeter)
                meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", MeterOffset, Grid.SceneLayer.NoLayer, new[]
                {
                    "meter_target",
                    "meter_fill",
                    "meter_frame",
                    "meter_OL"
                });
        }

        protected virtual bool IsConvertible(float dt)
        {
            var flag = true;
            var inputs = InOutItems.inputs;
            foreach (var inputItem in inputs)
            {
                var result = new List<GameObject>();
                var gameObjects = InStorage.Find(inputItem.tag, result);
                if (gameObjects != null)
                {
                    bool subflag = false;
                    var item1 = inputItem;
                    gameObjects.ForEach(item =>
                    {
                        if (subflag)
                        {
                            return;
                        }
                        var component = item.GetComponent<PrimaryElement>();
                        float num = item1.consumptionRate * dt;
                        subflag = (component.Mass >= num);
                    });
                    flag = subflag;
                }
                else
                {
                    flag = false;
                }
                if (!flag)
                {
                    break;
                }
            }
            return flag;
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);

            MeterUpdate(dt);
            GeneratePowerPre(dt, operational.IsOperational);
        }

        protected virtual void MeterUpdate(float dt)
        {
            if (!HasMeter) return;
            
            var ind = InOutItems.inputs[0];
            float percent = 0;
            if (InStorage.FindFirstWithMass(ind.tag) is PrimaryElement first)
                percent = first.Mass / ind.maxStoredMass;
            meter.SetPositionPercent(percent);
        }

        protected virtual void GeneratePowerPre(float dt, bool logicOn)
        {
            operational.SetFlag(wireConnectedFlag, CircuitID != ushort.MaxValue);

            if (!LogicOnCheckPre(logicOn)) return;

            bool flag = IsConvertible(dt);
            selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedResourceMass, !flag, InOutItems);
            operational.SetActive(flag);

            if (SetStatusPre(flag))
                GenerateWork(dt);
        }

        protected virtual bool LogicOnCheckPre(bool isOn) => isOn;

        protected virtual bool SetStatusPre(bool canConvert) => canConvert;

        protected virtual void GenerateWork(float dt)
        {
            PrimaryElement com = GetComponent<PrimaryElement>();
            Vector3 now = transform.GetPosition();
            int x, max;
            for (x = 0, max = InOutItems.inputs.Length; x < max; x++)
                ConsumedInput(InOutItems.inputs[x], dt);
            for (x = 0, max = InOutItems.outputs.Length; x < max; x++)
                EmitElements(com, now, InOutItems.outputs[x], dt);

            GenerateJoules(WattageRating * dt);
            selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
        }

        protected virtual void ConsumedInput(InputItem inp, float dt) =>
            InStorage.ConsumeIgnoringDisease(inp.tag, inp.consumptionRate * dt);

        protected virtual void EmitElements(PrimaryElement rootPrimaryElement, Vector3 now, OutputItem oti, float dt)
        {
            Element elbh = ElementLoader.FindElementByHash(oti.element);
            float temp = Mathf.Max(rootPrimaryElement.Temperature, oti.minTemperature), rate = oti.creationRate * dt;
            if (oti.store)
                if (elbh.IsGas)
                    OutStorage.AddGasChunk(oti.element, rate, temp, 255, 0, true);
                else if (elbh.IsLiquid)
                    OutStorage.AddLiquid(oti.element, rate, temp, 255, 0, true);
                else
                    OutStorage.Store(elbh.substance.SpawnResource(now, rate, temp, 255, 0), true);
            else
            {
                int pos = Grid.OffsetCell(Grid.PosToCell(now), oti.emitOffset);
                if (elbh.IsGas)
                    SimMessages.ModifyMass(pos, rate, 255, 0, CellEventLogger.Instance.EnergyGeneratorModifyMass, temp, oti.element);
                else if (elbh.IsLiquid)
                    FallingWater.instance.AddParticle(pos, (byte)ElementLoader.GetElementIndex(oti.element), rate, temp, 255, 0, true);
                else
                    elbh.substance.SpawnResource(Grid.CellToPosCCC(pos, Grid.SceneLayer.Front), rate, temp, 255, 0, true);
            }
        }
    }
}
