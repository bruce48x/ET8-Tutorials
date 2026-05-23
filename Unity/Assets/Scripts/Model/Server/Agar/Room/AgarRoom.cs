using System.Collections.Generic;

namespace ET.Server.Agar
{
    [ChildOf(typeof(AgarRoomManagerComponent))]
    public class AgarRoom: Entity, IAwake
    {
        public long RoomId;
        public readonly List<long> RealPlayerIds = new();
        public readonly List<long> AiPlayerIds = new();
        public readonly List<long> AllPlayerIds = new();
    }
}
