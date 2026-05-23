using System.Collections.Generic;

namespace ET.Server.Agar
{
    [ComponentOf(typeof(Scene))]
    public class AgarMatchComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public readonly List<AgarMatchPlayer> WaitPlayers = new();
        public const int MatchPlayerCount = 4;
        public const long MaxWaitTime = 5000;
    }

    [EnableClass]
    public class AgarMatchPlayer
    {
        public long PlayerId;
        public long JoinTime;
    }
}
