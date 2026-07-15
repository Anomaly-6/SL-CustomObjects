using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class CustomRoomIdentifierComponent : SchematicBlock
    {
        public string RoomName = "Unnamed";

        public override BlockType BlockType => BlockType.CustomRoomIdentifier;

        internal MeshFilter _filter;
        private MeshRenderer _renderer;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>
            {
                { "RoomName", RoomName },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            CustomRoomIdentifierComponent customRoomIdentifier = Instantiate(AssetDatabase.LoadAssetAtPath<CustomRoomIdentifierComponent>("Assets/Resources/Blocks/CustomRoomIdentifier.prefab"));

            gameObject = customRoomIdentifier.gameObject;

            if (block.Properties.TryGetValue("RoomName", out object roomName))
                customRoomIdentifier.RoomName = Convert.ToString(roomName);

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
            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
