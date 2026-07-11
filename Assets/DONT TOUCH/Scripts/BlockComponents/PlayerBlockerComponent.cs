using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class PlayerBlockerComponent : SchematicBlock
    {
        public PrimitiveType Type;
        public override BlockType BlockType => BlockType.PlayerBlocker;

        [Header("What can pass through the object?")]
        public bool ItemsAllowed = true;
        public bool BulletsAllowed = true;
        public List<DefaultRoleTypeId> Roles = new();

        internal MeshFilter _filter;
        private MeshRenderer _renderer;
        private Material _sharedTransparent;
        private PrimitiveType? _prevType;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>
            {
                { "PrimitiveType", Type },
                { nameof(ItemsAllowed), ItemsAllowed },
                { nameof(BulletsAllowed), BulletsAllowed },
                { nameof(Roles), Roles },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            PlayerBlockerComponent playerBlocker =
                Instantiate(
                    AssetDatabase.LoadAssetAtPath<PlayerBlockerComponent>(
                        "Assets/Resources/Blocks/PlayerBlocker.prefab"));
            gameObject = playerBlocker.gameObject;

            playerBlocker.Type = (PrimitiveType)Convert.ToInt32(block.Properties["PrimitiveType"]);
            if (block.Properties.TryGetValue(nameof(ItemsAllowed), out var itemsAllowedObj))
            {
                playerBlocker.ItemsAllowed = Convert.ToBoolean(itemsAllowedObj);
            }

            if (block.Properties.TryGetValue(nameof(BulletsAllowed), out var bulletsAllowedObj))
            {
                playerBlocker.BulletsAllowed = Convert.ToBoolean(bulletsAllowedObj);
            }

            if (block.Properties.TryGetValue(nameof(Roles), out var rolesObj))
            {
                foreach (var role in ((JArray)rolesObj).ToObject<List<DefaultRoleTypeId>>())
                {
                    playerBlocker.Roles.Add(role);
                }
            }

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