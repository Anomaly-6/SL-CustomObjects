using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class PlayerSpawnPointComponent : SchematicBlock
{
	public override BlockType BlockType { get; } = BlockType.PlayerSpawnPoint;
	public List<RoleTypeId> Roles = new();
	
	public override bool Compile(SchematicBlockData block, Schematic schematic)
	{
		block.BlockType = BlockType;
		block.Properties = new Dictionary<string, object>()
		{
			{ nameof(Roles), Roles },
		};
		return true;
	}
}