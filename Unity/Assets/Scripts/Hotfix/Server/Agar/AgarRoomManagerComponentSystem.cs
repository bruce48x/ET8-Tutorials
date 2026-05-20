namespace ET.Server.Agar
{
    [EntitySystemOf(typeof(AgarRoomManagerComponent))]
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
            self.Rooms.TryGetValue(roomId, out AgarRoom room);
            return room;
        }
    }
}