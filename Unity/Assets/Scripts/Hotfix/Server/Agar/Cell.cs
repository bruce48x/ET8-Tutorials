using Unity.Mathematics;

namespace ET.Server.Agar
{
    public class Cell : Entity, IAwake<long, float2, float>
    {
        public long OwnerPlayerId;
        public float2 Position;
        public float Radius;
        public float Mass;
    }
}