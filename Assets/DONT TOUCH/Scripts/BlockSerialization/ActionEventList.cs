using System;
using System.Collections.Generic;

[Serializable]
public class ActionEventList
{
    public string Id = string.Empty;
    public string DisplayName = "Actions";
    public List<ActionGame> Actions = new();

    public ActionEventList()
    {
    }

    public ActionEventList(string id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
        Actions = new List<ActionGame>();
    }

    public void EnsureDefaults(int fallbackIndex)
    {
        if (string.IsNullOrWhiteSpace(Id))
            Id = $"Event{fallbackIndex}";

        if (string.IsNullOrWhiteSpace(DisplayName))
            DisplayName = $"Event {fallbackIndex}";

        Actions ??= new List<ActionGame>();
    }
}
