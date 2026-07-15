using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class KillBoxComponent : SchematicBlock
    {
        public PrimitiveType Type;
        public string DeathReason = "Fatal blunt trauma; the body is badly mutilated and pulped.";

        public override BlockType BlockType => BlockType.KillBox;

        internal MeshFilter _filter;
        private MeshRenderer _renderer;
        private Material _sharedTransparent;
        private PrimitiveType? _prevType;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>
            {
                { "PrimitiveType", Type },
                { "DeathReason", DeathReason },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            KillBoxComponent interactable =
                Instantiate(AssetDatabase.LoadAssetAtPath<KillBoxComponent>("Assets/Resources/Blocks/KillBox.prefab"));

            gameObject = interactable.gameObject;

            interactable.Type = (PrimitiveType)Convert.ToInt32(block.Properties["PrimitiveType"]);

            if (block.Properties.TryGetValue("DeathReason", out object reason))
                interactable.DeathReason = Convert.ToString(reason);

            base.Decompile(ref gameObject, block, parent);
        }

        private void Start()
        {
            TryGetComponent(out _filter);
            TryGetComponent(out _renderer);

            _sharedTransparent = new Material((Material)Resources.Load("Materials/Transparent"));
            _renderer.sharedMaterial = _sharedTransparent;
            _renderer.sharedMaterial.color = new Color(1f, 0f, 0f, 0.1f);
        }

        private void Update()
        {
            _filter.hideFlags = HideFlags.HideInInspector;
            _renderer.hideFlags = HideFlags.HideInInspector;

            if (_prevType == Type)
                return;

            _prevType = Type;
            _filter.sharedMesh = PrimitiveMeshGetter.GetPrimitiveMesh(Type);
        }
    }
}
