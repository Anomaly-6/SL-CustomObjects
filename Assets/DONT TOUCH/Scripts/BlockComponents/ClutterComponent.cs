using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ClutterComponent : SchematicBlock
{
    public override BlockType BlockType { get; } = BlockType.Clutter;
    [Range(0f, 100f)]
    public float SpawnChance = 100;
    
    public override void Compile(SchematicBlockData block)
    {
        block.Properties = new Dictionary<string, object>
        {
            { "SpawnChance", SpawnChance },
        };
        base.Compile(block);
    }

    public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
    {
        var clutter = Create<ClutterComponent>("Assets/Resources/Blocks/Clutter.prefab");
        clutter.SpawnChance = Convert.ToSingle(block.Properties["SpawnChance"]);
        gameObject = clutter.gameObject;
        base.Decompile(ref gameObject, block, parent);
    }
}
