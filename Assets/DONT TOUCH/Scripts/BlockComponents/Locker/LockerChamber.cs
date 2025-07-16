using System.Collections.Generic;

[System.Serializable]
public class LockerChamber
{
    public List<ItemType> AcceptableItems;
    public bool IsOpen;
    public DoorPermissionFlags RequiredPermissions;
}