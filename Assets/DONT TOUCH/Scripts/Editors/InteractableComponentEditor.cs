using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableComponent))]
public class InteractableComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(
            serializedObject,
            "m_Script",
            nameof(InteractableComponent.ActionEvents));
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(6f);
        GUILayout.BeginHorizontal();
        ActionEventEditorWindow.DrawOpenButton(
            (MonoBehaviour)target,
            "Open Actions Editor",
            "Interactable Actions",
            GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
        ActionInfoWindow.DrawOpenButton("Action Info", GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
        GUILayout.EndHorizontal();
    }
}
