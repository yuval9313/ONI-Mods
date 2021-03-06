﻿using Database;
using Harmony;
using System.Collections.Generic;
using AdvancedGeneratos.Generators;
using static AdvancedGeneratos.Common.GeneratorCommonConstants;

namespace AdvancedGeneratos
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public sealed class GeneratedBuildings_LoadGeneratedBuildings_Patch
    {
        public static void Prefix()
        {
            SetString(RefinedCarbonGenerator.ID_UPPER, RefinedCarbonGenerator.NAME, RefinedCarbonGenerator.DESC, RefinedCarbonGenerator.EFFECT);
            SetString(ThermoelectricGenerator.ID_UPPER, ThermoelectricGenerator.NAME, ThermoelectricGenerator.DESC, ThermoelectricGenerator.EFFECT);
            SetString(NaphthaGenerator.ID_UPPER, NaphthaGenerator.NAME, NaphthaGenerator.DESC, NaphthaGenerator.EFFC);
            SetString(EcoFriendlyMethaneGenerator.ID_UPPER, EcoFriendlyMethaneGenerator.NAME, EcoFriendlyMethaneGenerator.DESC, EcoFriendlyMethaneGenerator.EFFECT);

            ModUtil.AddBuildingToPlanScreen(TabCategory, RefinedCarbonGenerator.ID);
            ModUtil.AddBuildingToPlanScreen(TabCategory, ThermoelectricGenerator.ID);
            ModUtil.AddBuildingToPlanScreen(TabCategory, NaphthaGenerator.ID);
            ModUtil.AddBuildingToPlanScreen(TabCategory, EcoFriendlyMethaneGenerator.ID);
        }

        private static void SetString(string path, string name, string desc, string effect)
        {
            Strings.Add($"{Kpath}{path}.NAME", name);
            Strings.Add($"{Kpath}{path}.DESC", desc);
            Strings.Add($"{Kpath}{path}.EFFECT", effect);
        }
    }

    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch(nameof(Db.Initialize))]
    public static class Db_Initialize_Patch
    {
        public static void Prefix()
        {
            Add("AdvancedPowerRegulation", RefinedCarbonGenerator.ID);
            Add("Plastics", NaphthaGenerator.ID);
            Add("RenewableEnergy", ThermoelectricGenerator.ID);
            Add("ImprovedCombustion", EcoFriendlyMethaneGenerator.ID);
        }

        private static void Add(string group, string id)
        {
            List<string> tech = new List<string>(Techs.TECH_GROUPING[group]) { id };
            Techs.TECH_GROUPING[group] = tech.ToArray();
        }
    }
}
