using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockComponents;
using DONT_TOUCH.Scripts.BlockSerialization;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode, SelectionBase]

    public class LightComponent : SchematicBlock
    {
        public override BlockType BlockType => BlockType.Light;

        [Tooltip("Будет ли свет выключаться при выключения света в комплексе?")]
        public bool Flicker;

        public DefaultFacilityZone FlickerZone;

        [HideInInspector] public LightType LightType;
        [HideInInspector] public float Intensity;
        [HideInInspector] public float Range;
        [HideInInspector] public float ShadowStrength;
        [HideInInspector] public LightShadows LightShadows;
        [HideInInspector] public float SpotAngle;
        [HideInInspector] public float InnerSpotAngle;
        [HideInInspector] public Color Color;

        public override void Compile(SchematicBlockData block)
        {
            TryGetComponent(out Light light);

            block.Properties = new Dictionary<string, object>
            {
                { "LightType", light.type },
                { "Color", ColorUtility.ToHtmlStringRGBA(light.color) },
                { "Intensity", light.intensity },
                { "Range", light.range },
                { "Shape", light.shape },
                { "SpotAngle", light.spotAngle },
                { "InnerSpotAngle", light.innerSpotAngle },
                { "ShadowStrength", light.shadowStrength },
                { "ShadowType", light.shadows },
                { nameof(Flicker), Flicker },
                { nameof(FlickerZone), FlickerZone },
            };

            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            LightType lightType = block.Properties.TryGetValue("LightType", out object objLightType)
                ? (LightType)Convert.ToInt32(objLightType)
                : LightType.Point;
            Light light = Create<GameObject>($"Assets/Resources/Blocks/Lights/{lightType} Light.prefab")
                .GetComponent<Light>();
            gameObject = light.gameObject;

            light.color = PrimitiveComponent.GetColorFromString(block.Properties["Color"].ToString());
            light.intensity = Convert.ToSingle(block.Properties["Intensity"]);
            light.range = Convert.ToSingle(block.Properties["Range"]);

            if (block.Properties.TryGetValue("Shadows", out object shadows))
            {
                // Backward compatibility
                light.shadows = Convert.ToBoolean(shadows) ? LightShadows.Soft : LightShadows.None;
            }
            else
            {
                light.shadows = (LightShadows)Convert.ToInt32(block.Properties["ShadowType"]);
                light.shape = (LightShape)Convert.ToInt32(block.Properties["Shape"]);
                light.spotAngle = Convert.ToSingle(block.Properties["SpotAngle"]);
                light.innerSpotAngle = Convert.ToSingle(block.Properties["InnerSpotAngle"]);
                light.shadowStrength = Convert.ToSingle(block.Properties["ShadowStrength"]);
            }

            if (block.Properties.TryGetValue(nameof(Flicker), out object flickerEnable))
            {
                Flicker = Convert.ToBoolean(flickerEnable);
            }

            if (block.Properties.TryGetValue(nameof(FlickerZone), out object flickerZone))
            {
                FlickerZone = (DefaultFacilityZone)Convert.ToInt32(flickerZone);
            }

            base.Decompile(ref gameObject, block, parent);
        }
    }
}