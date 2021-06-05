﻿using System.Collections.Generic;
using System.IO;
using AdvancedGenerators.Common;
using AdvancedGenerators.Generators;
using AdvancedGenerators.Models;
using Database;
using Harmony;
using MissileLib;
using MissileLib.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static AdvancedGenerators.Common.GeneratorCommonConstants;

// ReSharper disable InconsistentNaming

namespace AdvancedGenerators
{
    public class AdvancedGenerators
    {
        private static GeneratorsConfigs _configManager;
        public static class Mod_OnLoad
        {
            public static void OnLoad()
            {
                _configManager = InjectConfigurations();
                MissileLogger.InitLogger<FileLogger>();
            }
            
            private static GeneratorsConfigs InjectConfigurations()
            {
                var serializer = new JsonSerializer();
                using (var sw = new StreamReader(Path.Combine(Utilities.RunFolder ?? @".\", "config.json")))
                using (var reader = new JsonTextReader(sw))
                {
                    var json = JObject.Load(reader);
                    return JsonConvert.DeserializeObject<GeneratorsConfigs>(json.ToString());
                }
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        public static class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                MissileLogger.Log(" ----- Loading (DB Prefix) MOD: Advanced Generators v. 1.2.0 ----- ");
                // Vanilla prefers prefix for adding buildings
#if VANILLA
                SetupStrings();
#endif
            }

            public static void Postfix()
            {
                // DLC prefers postfix for adding buildings
#if SPACED_OUT
            SetupStrings();
#endif

                ModUtil.AddBuildingToPlanScreen(TabCategory, RefinedCarbonGenerator.Id);
                ModUtil.AddBuildingToPlanScreen(TabCategory, ThermoelectricGenerator.Id);
                ModUtil.AddBuildingToPlanScreen(TabCategory, NaphthaGenerator.Id);
                ModUtil.AddBuildingToPlanScreen(TabCategory, EcoFriendlyMethaneGenerator.Id);

                InsertToTechTree("AdvancedPowerRegulation", RefinedCarbonGenerator.Id);
                InsertToTechTree("Plastics", NaphthaGenerator.Id);
                InsertToTechTree("RenewableEnergy", ThermoelectricGenerator.Id);
                InsertToTechTree("ImprovedCombustion", EcoFriendlyMethaneGenerator.Id);
            }

            private static void InsertToTechTree(string group, string buildingId)
            {
#if VANILLA
                var tech = new List<string>(Techs.TECH_GROUPING[group]) {buildingId};
                Techs.TECH_GROUPING[group] = tech.ToArray();
#endif
#if SPACED_OUT
            var techObj = Db.Get().Techs.TryGet(group);
            if (techObj != null)
            {
                techObj.unlockedItemIDs.Add(buildingId);
            }
#endif
            }

            private static void SetupStrings()
            {
                SetString(RefinedCarbonGenerator.IdUpper, RefinedCarbonGenerator.Name,
                    RefinedCarbonGenerator.Description,
                    RefinedCarbonGenerator.Effect);
                SetupString<ThermoelectricGenerator>();
                SetString(NaphthaGenerator.IdUpper, NaphthaGenerator.Name, NaphthaGenerator.Description,
                    NaphthaGenerator.Effect);
                SetString(EcoFriendlyMethaneGenerator.IdUpper, EcoFriendlyMethaneGenerator.Name,
                    EcoFriendlyMethaneGenerator.Description, EcoFriendlyMethaneGenerator.Effect);
            }

            private static void SetupString<T>() where T: IAdvancedGeneratorConfig
            {
                var instance = Traverse.Create<T>();
                MissileLogger.Log(instance.Method("Name"));
                MissileLogger.Log(instance.Method("Field"));
                SetString(ThermoelectricGenerator.IdUpper,
                    instance.Method("Name").GetValue<string>(),
                    instance.Method("Description").GetValue<string>(),
                    instance.Method("Effect").GetValue<string>());
            }

            private static void SetString(string path, string name, string description, string effect)
            {
                Strings.Add($"{Kpath}{path}.NAME", name);
                Strings.Add($"{Kpath}{path}.DESC", description);
                Strings.Add($"{Kpath}{path}.EFFECT", effect);
            }
        }

        [HarmonyPatch(typeof(ThermoelectricGenerator))]
        [HarmonyPatch(nameof(ThermoelectricGenerator.CreateBuildingDef))]
        public static class ThermoelectricGenerator_CreateBuildingDef_Patch
        {
            public static void Prefix(ref ThermoelectricGenerator __instance)
            {
                var instance = Traverse.Create(__instance);
                instance.Field("_config").SetValue(_configManager.ThermoelectricGeneratorConfig);
            }
        }
    }
}