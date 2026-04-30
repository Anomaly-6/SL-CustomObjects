using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ActionGame
{
    public ActionType Type;
    public float ActionDelay;
    public string Value = string.Empty;

    [JsonIgnore] public GameObject Target;
    public BlockType BlockType;
    public int TargetId;
    public string Param = string.Empty;
    public AnimatorControllerParameterType ParamType;

    [JsonIgnore] public bool EditorIsExpanded = true;
    [JsonIgnore] public string Name = string.Empty;

    public void EnsureDefaults()
    {
        Value ??= string.Empty;
        Param ??= string.Empty;
        ActionDelay = Mathf.Max(0f, ActionDelay);

        // Очищаем параметры, не релевантные для текущего типа действия
        switch (Type)
        {
            case ActionType.Command:
            case ActionType.Audio:
                // Command и Audio используют Value, остальное очищаем
                TargetId = 0;
                Param = string.Empty;
                ParamType = default;
                BlockType = default;
                break;

            case ActionType.Animation:
                // Animation использует Target, Param, ParamType, Value
                BlockType = default;
                break;

            case ActionType.SetComponentProperty:
                // SetComponentProperty использует Target и Param (имя свойства), Value (новое значение)
                // TargetId заполняется при компиляции из Target
                ParamType = default;
                break;
            case ActionType.Destroy:
                ParamType = default;
                BlockType = default;
                Value = string.Empty;
                Param = string.Empty;
                break;
        }
    }
}