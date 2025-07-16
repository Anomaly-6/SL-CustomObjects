using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class ShootingTargetComponent : SchematicBlock
{
	public override BlockType BlockType { get; } = BlockType.ShootingTarget;
	public TargetType TargetType;
	
	public override bool Compile(SchematicBlockData block, Schematic schematic)
	{
		block.BlockType = BlockType;
		block.Properties = new Dictionary<string, object>()
		{
			{ nameof(TargetType), TargetType },
		};
		return true;
	}
}