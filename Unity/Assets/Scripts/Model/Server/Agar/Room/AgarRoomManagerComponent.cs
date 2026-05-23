using System.Collections.Generic;

namespace ET.Server.Agar
{
    [ComponentOf(typeof(Scene))]
    public class AgarRoomManagerComponent : Entity, IAwake, IDestroy
    {
        public readonly Dictionary<long, EntityRef<AgarRoom>> Rooms = new();
    }
}
