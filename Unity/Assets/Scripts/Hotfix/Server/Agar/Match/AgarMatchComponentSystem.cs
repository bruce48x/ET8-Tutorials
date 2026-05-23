using System.Collections.Generic;

namespace ET.Server.Agar
{
    [EntitySystemOf(typeof(AgarMatchComponent))]
    [FriendOf(typeof(AgarMatchComponent))]
    [FriendOf(typeof(AgarRoom))]
    public static partial class AgarMatchComponentSystem
    {
        [EntitySystem]
        private static void Awake(this AgarMatchComponent self)
        {
        }

        [EntitySystem]
        private static void Update(this AgarMatchComponent self)
        {
            self.TryStartMatch();
        }

        [EntitySystem]
        private static void Destroy(this AgarMatchComponent self)
        {
            self.WaitPlayers.Clear();
        }

        public static void Match(this AgarMatchComponent self, long playerId)
        {
            foreach (AgarMatchPlayer matchPlayer in self.WaitPlayers)
            {
                if (matchPlayer.PlayerId == playerId)
                {
                    return;
                }
            }

            self.WaitPlayers.Add(new AgarMatchPlayer
            {
                PlayerId = playerId,
                JoinTime = TimeInfo.Instance.ServerNow()
            });

            self.TryStartMatch();
        }

        private static void TryStartMatch(this AgarMatchComponent self)
        {
            if (self.WaitPlayers.Count == 0)
            {
                return;
            }

            bool enoughPlayers = self.WaitPlayers.Count >= AgarMatchComponent.MatchPlayerCount;
            bool firstPlayerTimeout = TimeInfo.Instance.ServerNow() - self.WaitPlayers[0].JoinTime >= AgarMatchComponent.MaxWaitTime;
            if (!enoughPlayers && !firstPlayerTimeout)
            {
                return;
            }

            List<long> realPlayerIds = new();
            while (realPlayerIds.Count < AgarMatchComponent.MatchPlayerCount && self.WaitPlayers.Count > 0)
            {
                realPlayerIds.Add(self.WaitPlayers[0].PlayerId);
                self.WaitPlayers.RemoveAt(0);
            }

            List<long> allPlayerIds = new(realPlayerIds);
            List<long> aiPlayerIds = new();
            while (allPlayerIds.Count < AgarMatchComponent.MatchPlayerCount)
            {
                long aiPlayerId = -IdGenerater.Instance.GenerateId();
                aiPlayerIds.Add(aiPlayerId);
                allPlayerIds.Add(aiPlayerId);
            }

            self.StartAgarRoom(realPlayerIds, aiPlayerIds, allPlayerIds);
        }

        private static void StartAgarRoom(
            this AgarMatchComponent self,
            List<long> realPlayerIds,
            List<long> aiPlayerIds,
            List<long> allPlayerIds)
        {
            long roomId = IdGenerater.Instance.GenerateId();
            AgarRoom room = self.Root().GetComponent<AgarRoomManagerComponent>().CreateRoom(roomId);
            room.RealPlayerIds.AddRange(realPlayerIds);
            room.AiPlayerIds.AddRange(aiPlayerIds);
            room.AllPlayerIds.AddRange(allPlayerIds);

            foreach (long playerId in allPlayerIds)
            {
                room.CreatePlayerCell(playerId);
            }

            MessageLocationSenderComponent messageLocationSenderComponent = self.Root().GetComponent<MessageLocationSenderComponent>();

            foreach (long playerId in realPlayerIds)
            {
                Match2G_AgarMatchSuccess matchSuccess = Match2G_AgarMatchSuccess.Create();
                matchSuccess.RoomId = roomId;
                messageLocationSenderComponent.Get(LocationType.Player).Send(playerId, matchSuccess);
            }
        }
    }
}
