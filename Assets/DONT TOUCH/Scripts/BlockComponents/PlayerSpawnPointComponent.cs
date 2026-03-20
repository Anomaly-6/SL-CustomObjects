using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class PlayerSpawnPointComponent : SchematicBlock
{
	public override BlockType BlockType { get; } = BlockType.PlayerSpawnPoint;
	public List<DefualtRoleTypeId> Roles = new();
	
	public override void Compile(SchematicBlockData block)
	{
		block.Properties = new Dictionary<string, object>()
		{
			{ nameof(Roles), Roles },
		};
		base.Compile(block);
	}

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
		PlayerSpawnPointComponent spawnPoint = Create<PlayerSpawnPointComponent>("Assets/Resources/Blocks/SpawnPoint.prefab");
		gameObject = spawnPoint.gameObject;
		foreach (var role in ((JArray)block.Properties["Roles"]).ToObject<List<DefualtRoleTypeId>>())
		{
			spawnPoint.Roles.Add(role);
		}		
		base.Decompile(ref gameObject, block, parent);
	}
}