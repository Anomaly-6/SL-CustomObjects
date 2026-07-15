using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class CustomZoneIdentifierComponent : SchematicBlock
    {
        public string ZoneName = "Unnamed";

        public override BlockType BlockType => BlockType.CustomZoneIdentifier;

        internal MeshFilter _filter;
        private MeshRenderer _renderer;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>
            {
                { "ZoneName", ZoneName },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            CustomZoneIdentifierComponent customZoneIdentifier =
                Instantiate(AssetDatabase.LoadAssetAtPath<CustomZoneIdentifierComponent>(
                    "Assets/Resources/Blocks/CustomZoneIdentifier.prefab"));

            gameObject = customZoneIdentifier.gameObject;

            if (block.Properties.TryGetValue("ZoneName", out object zoneName))
                customZoneIdentifier.ZoneName = Convert.ToString(zoneName);

            base.Decompile(ref gameObject, block, parent);
        }

        private void Start()
        {
            TryGetComponent(out _filter);
            TryGetComponent(out _renderer);

            if (_renderer != null)
                _renderer.enabled = false;
        }

        private void Update()
        {
            if (_filter != null)
                _filter.hideFlags = HideFlags.HideInInspector;

            if (_renderer != null)
                _renderer.hideFlags = HideFlags.HideInInspector;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
