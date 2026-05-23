using System.Collections.Generic;

namespace ET.Server.Agar
{
    [ComponentOf(typeof(AgarRoom))]
    public class CellComponent : Entity, IAwake, IDestroy
    {
        public readonly Dictionary<long, EntityRef<Cell>> Cells = new();
        public readonly Dictionary<long, EntityRef<Cell>> PlayerCells = new();
        public readonly List<long> FoodCellIds = new();
    }
}
