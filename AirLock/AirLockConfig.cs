using TUNING;
using UnityEngine;

public class AirLockConfig : PressureDoorConfig
{
	public new const string ID = "AirLock";

	public override BuildingDef CreateBuildingDef()
	{
		int width = 1;
		int height = 2;
		string anim = "door_external_kanim";
		int hitpoints = 60;
		float construction_time = 120f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] aLL_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Tile;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 1f);
        buildingDef.Overheatable = false;
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 240;
        buildingDef.Floodable = false;
        buildingDef.Entombable = false;
        buildingDef.IsFoundation = true;
        buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
        buildingDef.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(0, 0));
        SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Open_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Close_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

    public override void DoPostConfigureComplete(GameObject go)
    {
        base.DoPostConfigureComplete(go);
        Door door = go.AddOrGet<Door>();
        door.unpoweredAnimSpeed = 0.45f;
        door.poweredAnimSpeed = 5f;
    }
}
