using UnityEngine;

[ExecuteInEditMode, SelectionBase]
public class CapybaraComponent : SchematicBlock
{
	public override BlockType BlockType { get; } = BlockType.Capybara;

	public override bool Compile(SchematicBlockData block, Schematic schematic)
	{
		block.BlockType = BlockType.Capybara;
		return true;
	}
}