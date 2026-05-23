namespace ET.Server.Agar
{
    [EntitySystemOf(typeof(AgarRoomManagerComponent))]
    [FriendOf(typeof(AgarRoomManagerComponent))]
    [FriendOf(typeof(AgarRoom))]
    public static partial class AgarRoomManagerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this AgarRoomManagerComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this AgarRoomManagerComponent self)
        {
            self.Rooms.Clear();
            self.PlayerRoomIds.Clear();
        }

        public static AgarRoom CreateRoom(this AgarRoomManagerComponent self, long roomId)
        {
            AgarRoom room = self.AddChild<AgarRoom>();
            room.RoomId = roomId;

            room.AddComponent<CellComponent>();

            self.Rooms.Add(roomId, room);
            return room;
        }

        public static AgarRoom GetRoom(this AgarRoomManagerComponent self, long roomId)
        {
            self.Rooms.TryGetValue(roomId, out EntityRef<AgarRoom> room);
            return room;
        }

        public static void BindPlayerRoom(this AgarRoomManagerComponent self, long playerId, long roomId)
        {
            self.PlayerRoomIds[playerId] = roomId;
        }

        public static AgarRoom GetRoomByPlayer(this AgarRoomManagerComponent self, long playerId)
        {
            if (!self.PlayerRoomIds.TryGetValue(playerId, out long roomId))
            {
                return null;
            }

            return self.GetRoom(roomId);
        }

        public static void RemoveRoom(this AgarRoomManagerComponent self, long roomId)
        {
            if (!self.Rooms.Remove(roomId, out EntityRef<AgarRoom> roomRef))
            {
                return;
            }

            AgarRoom room = roomRef;
            if (room != null)
            {
                foreach (long playerId in room.RealPlayerIds)
                {
                    self.PlayerRoomIds.Remove(playerId);
                }
            }

            room?.Dispose();
        }
    }
}
