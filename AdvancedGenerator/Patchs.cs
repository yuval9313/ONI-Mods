using Database;
using Harmony;
using System.Collections.Generic;
using AdvancedGenerators.Generators;
using AdvancedGenerators.Generators.ThermoelectricGenerator;
using static AdvancedGenerators.Common.GeneratorCommonConstants;
// ReSharper disable InconsistentNaming

namespace AdvancedGenerators
{
    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public sealed class GeneratedBuildings_LoadGeneratedBuildings_Pat1ch
    {
        public static void Prefix()
        {
            SetString(RefinedCarbonGenerator.ID_UPPER, RefinedCarbonGenerator.NAME, RefinedCarbonGenerator.DESC, RefinedCarbonGenerator.EFFECT);
            SetString(ThermoelectricGenerator.IdUpper, ThermoelectricGenerator.Name, ThermoelectricGenerator.Description, ThermoelectricGenerator.Effect);
            SetString(NaphthaGenerator.ID_UPPER, NaphthaGenerator.NAME, NaphthaGenerator.DESC, NaphthaGenerator.EFFC);
            SetString(EcoFriendlyMethaneGenerator.ID_UPPER, EcoFriendlyMethaneGenerator.NAME, EcoFriendlyMethaneGenerator.DESC, EcoFriendlyMethaneGenerator.EFFECT);

            ModUtil.AddBuildingToPlanScreen(TabCategory, RefinedCarbonGenerator.ID);
            ModUtil.AddBuildingToPlanScreen(TabCategory, ThermoelectricGenerator.Id);
            ModUtil.AddBuildingToPlanScreen(TabCategory, NaphthaGenerator.ID);
            ModUtil.AddBuildingToPlanScreen(TabCategory, EcoFriendlyMethaneGenerator.ID);
        }

        private static void SetString(string path, string name, string description, string effect)
        {
            Strings.Add($"{Kpath}{path}.NAME", name);
            Strings.Add($"{Kpath}{path}.DESC", description);
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
            Add("RenewableEnergy", ThermoelectricGenerator.Id);
            Add("ImprovedCombustion", EcoFriendlyMethaneGenerator.ID);
        }

        private static void Add(string group, string id)
        {
            var tech = new List<string>(Techs.TECH_GROUPING[group]) { id };
            Techs.TECH_GROUPING[group] = tech.ToArray();
        }
    }
}
