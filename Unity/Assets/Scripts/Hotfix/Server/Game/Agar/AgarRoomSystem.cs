using Unity.Mathematics;

namespace ET.Game.Agar
{
    [EntitySystemOf(typeof(AgarRoom))]
    public static partial class AgarRoomSystem
    {
        [EntitySystem]
        private static void Awake(this AgarRoom self)
        {
        }

        public static Cell CreatePlayerCell(this AgarRoom self, long playerId)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();

            return cellComponent.CreateCell(
                playerId,
                new float2(0, 0),
                10f);
        }
    }
}