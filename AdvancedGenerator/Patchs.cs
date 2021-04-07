﻿using System.Collections.Generic;
using Harmony;
using AdvancedGenerators.Generators;
using Database;
using static AdvancedGenerators.Common.GeneratorCommonConstants;
// ReSharper disable InconsistentNaming

namespace AdvancedGenerators
{
    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch(nameof(Db.Initialize))]
    public static class Db_Initialize_Patch
    {
        public static void Prefix()
        {
            Debug.Log(" ----- Loading (DB Prefix) MOD: Advanced Generators v. 1.1 ----- ");
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
            ModUtil.AddBuildingToPlanScreen(TabCategory, NaphthaGenerator.ID);
            ModUtil.AddBuildingToPlanScreen(TabCategory, EcoFriendlyMethaneGenerator.ID);
            
            InsertToTechTree("AdvancedPowerRegulation", RefinedCarbonGenerator.Id);
            InsertToTechTree("Plastics", NaphthaGenerator.ID);
            InsertToTechTree("RenewableEnergy", ThermoelectricGenerator.Id);
            InsertToTechTree("ImprovedCombustion", EcoFriendlyMethaneGenerator.ID);
        }
        
        private static void InsertToTechTree(string group, string buildingId)
        {
#if VANILLA
            var tech = new List<string>(Techs.TECH_GROUPING[group]) { buildingId };
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
            SetString(RefinedCarbonGenerator.IdUpper, RefinedCarbonGenerator.Name, RefinedCarbonGenerator.Description, RefinedCarbonGenerator.Effect);
            SetString(ThermoelectricGenerator.IdUpper, ThermoelectricGenerator.Name, ThermoelectricGenerator.Description, ThermoelectricGenerator.Effect);
            SetString(NaphthaGenerator.ID_UPPER, NaphthaGenerator.NAME, NaphthaGenerator.DESC, NaphthaGenerator.EFFC);
            SetString(EcoFriendlyMethaneGenerator.ID_UPPER, EcoFriendlyMethaneGenerator.NAME, EcoFriendlyMethaneGenerator.DESC, EcoFriendlyMethaneGenerator.EFFECT);
        }
        
        private static void SetString(string path, string name, string description, string effect)
        {
            Strings.Add($"{Kpath}{path}.NAME", name);
            Strings.Add($"{Kpath}{path}.DESC", description);
            Strings.Add($"{Kpath}{path}.EFFECT", effect);
        }
        
    }
}
