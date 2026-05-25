using System.Collections.Generic;

namespace ET.Server.Agar
{
    [ChildOf(typeof(AgarRoomManagerComponent))]
    public class AgarRoom: Entity, IAwake, IUpdate, IDestroy
    {
        public const long MatchDuration = 60 * 1000;
        public const long SyncInterval = 200;
        public const float ArenaHalfSize = 80f;
        public const int TargetFoodCount = 80;
        public const float InitialPlayerRadius = 6f;
        public const float FoodRadius = 1.6f;
        public const float MinEatRadiusRatio = 1.08f;

        public long RoomId;
        public long StartTime;
        public long EndTime;
        public long LastUpdateTime;
        public long LastSyncTime;
        public bool IsFinished;

        public readonly List<long> RealPlayerIds = new();
        public readonly List<long> AiPlayerIds = new();
        public readonly List<long> AllPlayerIds = new();
        public readonly Dictionary<long, string> RealPlayerAccounts = new();
    }
}
