using Unity.Mathematics;

namespace ET.Server.Agar
{
    [EntitySystemOf(typeof(CellComponent))]
    [FriendOf(typeof(CellComponent))]
    [FriendOf(typeof(Cell))]
    public static partial class CellComponentSystem
    {
        [EntitySystem]
        private static void Awake(this CellComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this CellComponent self)
        {
            self.Cells.Clear();
            self.PlayerCells.Clear();
            self.FoodCellIds.Clear();
        }

        public static Cell CreateCell(
            this CellComponent self,
            long ownerPlayerId,
            float2 position,
            float radius)
        {
            Cell cell = self.AddChild<Cell, long, float2, float>(
                ownerPlayerId,
                position,
                radius);

            self.Cells.Add(cell.Id, cell);
            if (ownerPlayerId == 0)
            {
                self.FoodCellIds.Add(cell.Id);
            }
            else
            {
                self.PlayerCells[ownerPlayerId] = cell;
            }

            return cell;
        }

        public static void RemoveCell(this CellComponent self, long cellId)
        {
            if (!self.Cells.Remove(cellId, out EntityRef<Cell> cellRef))
            {
                return;
            }

            Cell cell = cellRef;
            if (cell.OwnerPlayerId == 0)
            {
                self.FoodCellIds.Remove(cellId);
            }
            else
            {
                self.PlayerCells.Remove(cell.OwnerPlayerId);
            }

            cell.Dispose();
        }

        public static Cell GetPlayerCell(this CellComponent self, long playerId)
        {
            self.PlayerCells.TryGetValue(playerId, out EntityRef<Cell> cell);
            return cell;
        }
    }

    [EntitySystemOf(typeof(Cell))]
    [FriendOf(typeof(Cell))]
    public static partial class CellSystem
    {
        [EntitySystem]
        private static void Awake(this Cell self, long ownerPlayerId, float2 position, float radius)
        {
            self.OwnerPlayerId = ownerPlayerId;
            self.Position = position;
            self.Direction = new float2(0, 0);
            self.Radius = radius;
            self.Mass = radius * radius;
        }

        public static void AddMass(this Cell self, float mass)
        {
            self.Mass += mass;
            self.Radius = math.sqrt(self.Mass);
        }
    }
}
