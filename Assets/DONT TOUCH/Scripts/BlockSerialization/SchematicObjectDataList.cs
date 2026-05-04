using System;
using System.Collections.Generic;

namespace DONT_TOUCH.Scripts.BlockSerialization
{
    [Serializable]
    public class SchematicObjectDataList
    {
        public int RootObjectId { get; set; }

        public List<SchematicBlockData> Blocks { get; set; } = new List<SchematicBlockData>();
    }
}
