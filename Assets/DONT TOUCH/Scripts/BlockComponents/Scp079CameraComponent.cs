using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class Scp079CameraComponent : SchematicBlock
{
	public override BlockType BlockType { get; } = BlockType.Camera;
	public CameraType CameraType;
	public string Label;

	public override bool Compile(SchematicBlockData block, Schematic _)
	{
		block.BlockType = BlockType;
		block.Properties = new Dictionary<string, object>()
		{
			{ nameof(CameraType), CameraType },
			{ nameof(Label), Label }
		};
		return true;
	}
}