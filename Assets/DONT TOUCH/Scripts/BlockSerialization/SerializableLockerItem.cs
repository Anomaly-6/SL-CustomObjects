using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableLockerItem
{
    public SerializableLockerItem()
    {
    }
    
    public SerializableLockerItem(LockerItem lockerItem)
    {
        TargetItem = lockerItem.TargetItem.ToString();
        RemainingUses = lockerItem.RemainingUses;
        ProbabilityPoints = lockerItem.ProbabilityPoints;
        MinPerChamber = lockerItem.MinPerChamber;
        MaxPerChamber = lockerItem.MaxPerChamber;
    }
    
    public string TargetItem { get; set; }
    
    [Min(0)]
    public int RemainingUses = 1;
    
    [Range(0, 100)]
    public int ProbabilityPoints = 100;

    [Min(0)]
    public int MinPerChamber = 1;
    
    [Min(1)]
    public int MaxPerChamber = 10;
}