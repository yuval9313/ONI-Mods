using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using static Door;

namespace AirLock
{

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
	internal class AirLock_GeneratedBuildings_LoadGeneratedBuildings
	{
		private static void Prefix()
		{
			Debug.Log(" === AirLock_GeneratedBuildings_LoadGeneratedBuildings Prefix === ");
			Strings.Add("STRINGS.BUILDINGS.PREFABS.AIRLOCK.NAME", "AirLock");
			Strings.Add("STRINGS.BUILDINGS.PREFABS.AIRLOCK.DESC", "This door doesn't allow gases or liquids to flow.");
			Strings.Add("STRINGS.BUILDINGS.PREFABS.AIRLOCK.EFFECT", "");

			ModUtil.AddBuildingToPlanScreen("Utilities", AirLockConfig.ID);

		}
	
	}

	[HarmonyPatch(typeof(Db), "Initialize")]
	internal class AirLock_Db_Initialize
	{
		private static void Prefix(Db __instance)
		{
			Debug.Log(" === AirLock_Db_Initialize loaded === ");
			List<string> ls = new List<string>((string[])Database.Techs.TECH_GROUPING["DupeTrafficControl"]);
			ls.Add(AirLockConfig.ID);
			Database.Techs.TECH_GROUPING["DupeTrafficControl"] = (string[])ls.ToArray();
		}
	}

    [HarmonyPatch(typeof(Door), "SetSimState")]
    internal class FunctionalAirlocks_Door_SetSimState
    {
        private static bool Prefix(Door __instance, bool is_door_open, IList<int> cells)
        {
            // If the attached gameobject doesn't exist, exit here
            if (__instance.gameObject == null)
            { return true; }

            Debug.Log($"The prefab ID is: {__instance.PrefabID()}");

            // Get the door type
            Door.DoorType doorType = __instance.doorType;
            if (__instance.PrefabID() != AirLockConfig.ID) 
            { return true; }
            Debug.Log($"DoorType is: {doorType}");
            Debug.Log($"The instance tag is: {__instance.tag}");
            Debug.Log($"The gameobject is: {__instance.gameObject}");
            Debug.Log($"The name is: {__instance.name}");
            Debug.Log($"{__instance.ToString()}");

            // Get the door control state
            Door.ControlState controlState = Traverse.Create(__instance).Field("controlState").GetValue<Door.ControlState>();

            // Get the mass of the door (per cell)
            PrimaryElement element = __instance.GetComponent<PrimaryElement>();
            float mass = element.Mass / cells.Count;

            foreach (var gameCell in cells)
            {
                Debug.Log($"Cell is: {gameCell}");
                SimMessages.SetCellProperties(gameCell, 4);

                float temperature = element.Temperature;
                SimHashes elementID = element.ElementID;

                if (is_door_open)
                {
                    if (controlState == Door.ControlState.Auto)
                    {
                        MethodInfo method_opened = AccessTools.Method(typeof(Door), "OnSimDoorOpened", null, null);
                        System.Action cb_opened = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_opened);
                        HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_opened, false));
                        CellElementEvent doorOpen = CellEventLogger.Instance.DoorOpen;
                        SimMessages.ReplaceAndDisplaceElement(gameCell, elementID, doorOpen, mass, temperature, byte.MaxValue, 0, handle.index);
                    }
                }
                else
                {
                    MethodInfo method_closed = AccessTools.Method(typeof(Door), "OnSimDoorClosed", null, null);
                    System.Action cb_closed = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_closed);
                    HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_closed, false));
                    CellElementEvent doorClose = CellEventLogger.Instance.DoorClose;
                    SimMessages.ReplaceAndDisplaceElement(gameCell, elementID, doorClose, mass, temperature, byte.MaxValue, 0, handle.index);
                }
            }

            return false;
        }

        [HarmonyPatch(typeof(Door), "OnPrefabInit")]
        internal class FunctionalAirlocks_Door_OnPrefabInit
        {
            private static void Postfix(ref Door __instance)
            {
                Debug.Log("Adding anim override");
                __instance.overrideAnims = new KAnimFile[]
                {
                    Assets.GetAnim("anim_use_remote_kanim")
                };
            }
        }

        [HarmonyPatch(typeof(Door), "OnCleanUp")]
        internal class FunctionalAirlocks_Door_OnCleanUp
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                List<CodeInstruction> code = instr.ToList();
                foreach (CodeInstruction codeInstruction in code)
                {
                    if (codeInstruction.opcode == OpCodes.Ldc_I4_S && (sbyte)codeInstruction.operand == 12)
                    { codeInstruction.operand = 13; Debug.Log("Made 13"); }
                    yield return codeInstruction;
                }
            }
        }

    }
}
