using DONT_TOUCH.Scripts;
using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(CullingParentComponent))]

    public class CullingParentComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var cullingParent = (CullingParentComponent)target;
            DrawDefaultInspector();

            GUILayout.Label(
                $"<color=white>Number of blocks: <b>{cullingParent.GetComponentsInChildren<SchematicBlock>().Length - 1}</b></color>",
                SchematicManager.UnityRichTextStyle);
        }
    }
}