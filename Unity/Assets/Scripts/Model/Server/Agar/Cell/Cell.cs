using Unity.Mathematics;

namespace ET.Server.Agar
{
    [ChildOf(typeof(CellComponent))]
    public class Cell : Entity, IAwake<long, float2, float>
    {
        public long OwnerPlayerId;
        public float2 Position;
        public float2 Direction;
        public float Radius;
        public float Mass;

        public bool IsFood => this.OwnerPlayerId == 0;
    }
}
