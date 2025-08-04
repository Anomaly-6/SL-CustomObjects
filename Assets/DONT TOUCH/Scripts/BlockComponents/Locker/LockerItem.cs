using System;
using UnityEngine;

[Serializable]
public class LockerItem
{
    public LockerItem()
    {
    }

    public LockerItem(SerializableLockerItem serializableLockerItem)
    {
        if (Enum.TryParse(serializableLockerItem.TargetItem, out ItemType itemType))
        {
            TargetItem = itemType;
        }
        
        RemainingUses = serializableLockerItem.RemainingUses;
        ProbabilityPoints = serializableLockerItem.ProbabilityPoints;
        MinPerChamber = serializableLockerItem.MinPerChamber;
        MaxPerChamber = serializableLockerItem.MaxPerChamber;
        //Count = serializableLockerItem.Count;
        // Attachments = serializableLockerItem.Attachments;
        //Chance = serializableLockerItem.Chance;
    }
    
    [Tooltip("The ItemType of this pickup.")]
    public ItemType TargetItem;

    [Min(0)]
    public int RemainingUses = 1;
    
    [Min(0)]
    public int ProbabilityPoints = 100;

    [Min(0)]
    public int MinPerChamber = 1;
    
    [Min(0)]
    public int MaxPerChamber = 10;
    //public uint Count = 1;

    // [ReorderableList]
    // public List<AttachmentName> Attachments = new List<AttachmentName>();

    //public float Chance = 100;
}