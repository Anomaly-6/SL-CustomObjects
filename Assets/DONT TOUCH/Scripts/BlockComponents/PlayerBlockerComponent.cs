using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class PlayerBlockerComponent : SchematicBlock
    {
        public PrimitiveType Type;
        public override BlockType BlockType => BlockType.PlayerBlocker;

        internal MeshFilter _filter;
        private MeshRenderer _renderer;
        private Material _sharedTransparent;
        private PrimitiveType? _prevType;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>
            {
                { "PrimitiveType", Type },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            PlayerBlockerComponent interactable =
                Instantiate(AssetDatabase.LoadAssetAtPath<PlayerBlockerComponent>("Assets/Resources/Blocks/PlayerBlocker.prefab"));
            gameObject = interactable.gameObject;

            interactable.Type = (PrimitiveType)Convert.ToInt32(block.Properties["PrimitiveType"]);

            base.Decompile(ref gameObject, block, parent);
        }

        private void Start()
        {
            TryGetComponent(out _filter);
            TryGetComponent(out _renderer);
            _sharedTransparent = new Material((Material)Resources.Load("Materials/Transparent"));
            _renderer.sharedMaterial = _sharedTransparent;
            _renderer.sharedMaterial.color = new Color(0, 1, 0, 0.1f);
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