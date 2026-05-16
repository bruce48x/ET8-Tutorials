using Unity.Mathematics;

namespace ET.Game.Agar
{
    [EntitySystemOf(typeof(CellComponent))]
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
            return cell;
        }

        public static void RemoveCell(this CellComponent self, long cellId)
        {
            if (!self.Cells.Remove(cellId, out Cell cell))
            {
                return;
            }

            cell.Dispose();
        }
    }

    [EntitySystemOf(typeof(Cell))]
    public static partial class CellSystem
    {
        [EntitySystem]
        private static void Awake(this Cell self, long ownerPlayerId, float2 position, float radius)
        {
            self.OwnerPlayerId = ownerPlayerId;
            self.Position = position;
            self.Radius = radius;
            self.Mass = radius * radius;
        }
    }
}