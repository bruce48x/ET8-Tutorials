using System.Collections.Generic;

namespace ET.Server.Agar
{
    public class AgarRoomManagerComponent : Entity, IAwake, IDestroy
    {
        public readonly Dictionary<long, AgarRoom> Rooms = new();
    }
}