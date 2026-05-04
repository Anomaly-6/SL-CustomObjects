using System.Collections.Generic;
using DONT_TOUCH.Enums;

namespace DONT_TOUCH.Scripts.BlockComponents.Locker
{
    [System.Serializable]
    public class LockerChamber
    {
        public List<ItemType> AcceptableItems;
        public bool IsOpen;
        public DoorPermissionFlags RequiredPermissions;
    }
}