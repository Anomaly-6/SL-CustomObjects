using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public class AudioPlayerComponent : SchematicBlock
    {
        public override BlockType BlockType { get; } = BlockType.AudioPlayer;
        [Space] public string FileName;

        public bool IsShortClip;
        public bool PlayOnSpawn;
        public bool Loop;
        public bool IsSpatial = true;
        [Min(0)] public float Volume = 1f;
        [Min(0)] public float MinDistance = 1f;
        [Min(0)] public float MaxDistance = 15f;
        public float Speed = 1f;
        [HideInInspector] public bool Pause;

        public override void Compile(SchematicBlockData block)
        {
            block.Properties = new Dictionary<string, object>()
            {
                { "FileName", FileName },
                { "IsShortClip", IsShortClip },
                { "PlayOnSpawn", PlayOnSpawn },
                { "Loop", Loop },
                { "IsSpatial", IsSpatial },
                { "Volume", Volume },
                { "MinDistance", MinDistance },
                { "MaxDistance", MaxDistance },
                { "Speed", Speed },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            AudioPlayerComponent audioPlayerComponent = Create<AudioPlayerComponent>($"Assets/Resources/Blocks/AudioPlayer.prefab");
            gameObject = audioPlayerComponent.gameObject;

            if (block.Properties.TryGetValue("FileName", out object fileNameObj))
                audioPlayerComponent.FileName = Convert.ToString(fileNameObj);
            
            if (block.Properties.TryGetValue("IsShortClip", out object isShortClipObj))
                audioPlayerComponent.IsShortClip = Convert.ToBoolean(isShortClipObj);
            
            if (block.Properties.TryGetValue("PlayOnSpawn", out object playOnSpawnObj))
                audioPlayerComponent.PlayOnSpawn = Convert.ToBoolean(playOnSpawnObj);
            
            if (block.Properties.TryGetValue("Loop", out object loopObj))
                audioPlayerComponent.Loop = Convert.ToBoolean(loopObj);
            
            if (block.Properties.TryGetValue("IsSpatial", out object isSpatialObj))
                audioPlayerComponent.IsSpatial = Convert.ToBoolean(isSpatialObj);
            
            if (block.Properties.TryGetValue("Volume", out object volumeObj))
                audioPlayerComponent.Volume = Convert.ToSingle(volumeObj);
            
            if (block.Properties.TryGetValue("MinDistance", out object minDistanceObj))
                audioPlayerComponent.MinDistance = Convert.ToSingle(minDistanceObj);
            
            if (block.Properties.TryGetValue("MaxDistance", out object maxDistanceObj))
                audioPlayerComponent.MaxDistance = Convert.ToSingle(maxDistanceObj);
            
            if (block.Properties.TryGetValue("Speed", out object speedObj))
                audioPlayerComponent.Speed = Convert.ToSingle(speedObj);
            
            base.Decompile(ref gameObject, block, parent);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, MaxDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, MinDistance);
        }
    }
}