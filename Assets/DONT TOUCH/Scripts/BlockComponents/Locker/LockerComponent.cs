using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class LockerComponent : SchematicBlock
{
	public List<LockerChamber> Chambers = new();
	public List<LockerItem> Loot = new();

	public LockerType LockerType;
	public override BlockType BlockType => BlockType.Locker;

	public override bool Compile(SchematicBlockData block, Schematic _)
	{
		block.BlockType = BlockType.Locker;
		List<string> jsonLoot = new(Loot.Count);
		List<string> jsonChamber = new(Chambers.Count);
		foreach (var chamber in Chambers)
		{
			jsonChamber.Add(JsonConvert.SerializeObject(chamber));
		}
		foreach (var loot in Loot)
		{
			jsonLoot.Add(JsonConvert.SerializeObject(loot));	
		}

		block.Properties = new Dictionary<string, object>()
		{
			{ "LockerType", LockerType },	
			{ "Chambers", jsonChamber },
			{ "Loot", jsonLoot },
		};

		return true;
	}
}