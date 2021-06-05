using System.Collections.Generic;
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
        public static void Postfix()
        {
            Debug.Log(" ----- Loading MOD: Advanced Generators v. 1.2 ----- ");
            SetupStrings();
            
            ModUtil.AddBuildingToPlanScreen(TabCategory, RefinedCarbonGenerator.Id);
            ModUtil.AddBuildingToPlanScreen(TabCategory, ThermoelectricGenerator.Id);
            ModUtil.AddBuildingToPlanScreen(TabCategory, NaphthaGenerator.Id);
            ModUtil.AddBuildingToPlanScreen(TabCategory, EcoFriendlyMethaneGenerator.Id);
            
            InsertToTechTree("AdvancedPowerRegulation", RefinedCarbonGenerator.Id);
            InsertToTechTree("Plastics", NaphthaGenerator.Id);
            InsertToTechTree("RenewableEnergy", ThermoelectricGenerator.Id);
            InsertToTechTree("ImprovedCombustion", EcoFriendlyMethaneGenerator.Id);
        }
        
        private static void InsertToTechTree(string techId, string buildingId)
        {
            var tech_grouping = Traverse.Create(typeof(Techs))?.Field("TECH_GROUPING")?.GetValue<Dictionary<string, string[]>>();
            var isVanilla = tech_grouping != null;
            if (isVanilla)
            {
                if (tech_grouping.ContainsKey(techId))
                {
                    var techList = new List<string>(tech_grouping[techId]) { buildingId };
                    tech_grouping[techId] = techList.ToArray();
                }
                else
                    Debug.LogWarning($"Advanced Generators: Could not find '{techId}' tech in TECH_GROUPING.");
            }
            else
            {
                var tech = Db.Get()?.Techs.TryGet(techId);
                if (tech != null)
                {
                    Traverse.Create(tech)?.Field("unlockedItemIDs")?.GetValue<List<string>>()?.Add(buildingId);
                }
                else
                    Debug.LogWarning($"Advanced Generators: Could not find '{techId}' tech.");
            }
        }

        private static void SetupStrings()
        {
            SetString(RefinedCarbonGenerator.IdUpper, RefinedCarbonGenerator.Name, RefinedCarbonGenerator.Description, RefinedCarbonGenerator.Effect);
            SetString(ThermoelectricGenerator.IdUpper, ThermoelectricGenerator.Name, ThermoelectricGenerator.Description, ThermoelectricGenerator.Effect);
            SetString(NaphthaGenerator.IdUpper, NaphthaGenerator.Name, NaphthaGenerator.Description, NaphthaGenerator.Effect);
            SetString(EcoFriendlyMethaneGenerator.IdUpper, EcoFriendlyMethaneGenerator.Name, EcoFriendlyMethaneGenerator.Description, EcoFriendlyMethaneGenerator.Effect);
        }
        
        private static void SetString(string path, string name, string description, string effect)
        {
            Strings.Add($"{Kpath}{path}.NAME", name);
            Strings.Add($"{Kpath}{path}.DESC", description);
            Strings.Add($"{Kpath}{path}.EFFECT", effect);
        }
        
    }
}
