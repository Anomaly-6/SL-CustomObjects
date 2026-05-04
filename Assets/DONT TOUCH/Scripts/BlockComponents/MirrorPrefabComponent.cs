using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode, SelectionBase]
    public class MirrorPrefabComponent : SchematicBlock
    {
        public override BlockType BlockType { get; } = BlockType.MirrorPrefab;
        public MirrorPrefabType MirrorType = MirrorPrefabType.BrokenElectricalBox;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>
            {
                { "MirrorType", MirrorType },
            };
            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            MirrorPrefabType mirrorType = (MirrorPrefabType)Convert.ToInt32(block.Properties["MirrorType"]);
            MirrorPrefabComponent doorComponent = Create<MirrorPrefabComponent>($"Assets/Resources/Blocks/MirrorPrefabs/{mirrorType}.prefab");
            gameObject = doorComponent.gameObject;
            base.Decompile(ref gameObject, block, parent);
        }
    }
}