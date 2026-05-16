using System.Collections.Generic;

namespace ET.Game.Agar
{
    public class AgarRoomManagerComponent : Entity, IAwake, IDestroy
    {
        public readonly Dictionary<long, AgarRoom> Rooms = new();
    }
}