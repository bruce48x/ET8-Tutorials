using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server.Agar
{
    [EntitySystemOf(typeof(AgarRoom))]
    [FriendOf(typeof(AgarRoom))]
    [FriendOf(typeof(CellComponent))]
    [FriendOf(typeof(Cell))]
    public static partial class AgarRoomSystem
    {
        [EntitySystem]
        private static void Awake(this AgarRoom self)
        {
        }

        [EntitySystem]
        private static void Update(this AgarRoom self)
        {
            self.TickBattle();
        }

        [EntitySystem]
        private static void Destroy(this AgarRoom self)
        {
            self.RealPlayerIds.Clear();
            self.AiPlayerIds.Clear();
            self.AllPlayerIds.Clear();
        }

        public static void StartBattle(this AgarRoom self)
        {
            long now = TimeInfo.Instance.ServerNow();
            self.StartTime = now;
            self.EndTime = now + AgarRoom.MatchDuration;
            self.LastUpdateTime = now;
            self.LastSyncTime = 0;
            self.IsFinished = false;

            for (int i = 0; i < self.AllPlayerIds.Count; ++i)
            {
                self.CreatePlayerCell(self.AllPlayerIds[i], i, self.AllPlayerIds.Count);
            }

            self.RefillFood();
        }

        public static Cell CreatePlayerCell(this AgarRoom self, long playerId)
        {
            return self.CreatePlayerCell(playerId, 0, 1);
        }

        private static Cell CreatePlayerCell(this AgarRoom self, long playerId, int index, int count)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            float angle = count <= 1 ? 0 : math.PI * 2f * index / count;
            float spawnRadius = AgarRoom.ArenaHalfSize * 0.55f;
            float2 position = new(math.cos(angle) * spawnRadius, math.sin(angle) * spawnRadius);
            Cell cell = cellComponent.CreateCell(playerId, position, AgarRoom.InitialPlayerRadius);
            cell.Direction = playerId < 0 ? self.RandomDirection() : float2.zero;
            return cell;
        }

        private static void TickBattle(this AgarRoom self)
        {
            if (self.IsFinished || self.StartTime == 0)
            {
                return;
            }

            long now = TimeInfo.Instance.ServerNow();
            float deltaTime = math.min((now - self.LastUpdateTime) / 1000f, 0.1f);
            self.LastUpdateTime = now;

            self.UpdateCells(deltaTime);
            self.ResolveFoodEats();
            self.ResolvePlayerEats();
            self.RefillFood();

            if (now - self.LastSyncTime >= AgarRoom.SyncInterval)
            {
                self.BroadcastState();
            }

            if (now >= self.EndTime || self.GetAlivePlayerCount() <= 1)
            {
                self.FinishBattle();
            }
        }

        public static void SetPlayerDirection(this AgarRoom self, long playerId, float directionX, float directionY)
        {
            if (self.IsFinished)
            {
                return;
            }

            Cell cell = self.GetComponent<CellComponent>().GetPlayerCell(playerId);
            if (cell == null)
            {
                return;
            }

            float2 direction = new(directionX, directionY);
            cell.Direction = math.lengthsq(direction) > 0.001f ? math.normalize(direction) : float2.zero;
        }

        private static void UpdateCells(this AgarRoom self, float deltaTime)
        {
            if (deltaTime <= 0)
            {
                return;
            }

            CellComponent cellComponent = self.GetComponent<CellComponent>();
            foreach (long playerId in self.AllPlayerIds)
            {
                Cell cell = cellComponent.GetPlayerCell(playerId);
                if (cell == null)
                {
                    continue;
                }

                if (playerId < 0 && (math.lengthsq(cell.Direction) < 0.01f || RandomGenerator.RandomNumber(0, 100) < 4))
                {
                    cell.Direction = self.RandomDirection();
                }

                if (math.lengthsq(cell.Direction) < 0.001f)
                {
                    continue;
                }

                float speed = self.GetMoveSpeed(cell);
                cell.Position += math.normalize(cell.Direction) * speed * deltaTime;

                float max = AgarRoom.ArenaHalfSize - cell.Radius;
                if (cell.Position.x < -max || cell.Position.x > max)
                {
                    cell.Direction.x = -cell.Direction.x;
                }

                if (cell.Position.y < -max || cell.Position.y > max)
                {
                    cell.Direction.y = -cell.Direction.y;
                }

                cell.Position = math.clamp(cell.Position, new float2(-max, -max), new float2(max, max));
            }
        }

        private static float GetMoveSpeed(this AgarRoom self, Cell cell)
        {
            return math.max(6f, 28f - cell.Radius * 0.7f);
        }

        private static void ResolveFoodEats(this AgarRoom self)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            List<long> eatenFoodIds = new();

            foreach (long foodCellId in cellComponent.FoodCellIds)
            {
                if (!cellComponent.Cells.TryGetValue(foodCellId, out EntityRef<Cell> foodRef))
                {
                    continue;
                }

                Cell food = foodRef;
                Cell eater = self.FindFoodEater(food);
                if (eater == null)
                {
                    continue;
                }

                eater.AddMass(food.Mass);
                eatenFoodIds.Add(food.Id);
            }

            foreach (long foodCellId in eatenFoodIds)
            {
                cellComponent.RemoveCell(foodCellId);
            }
        }

        private static Cell FindFoodEater(this AgarRoom self, Cell food)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            Cell bestEater = null;

            foreach ((_, EntityRef<Cell> cellRef) in cellComponent.PlayerCells)
            {
                Cell playerCell = cellRef;
                float eatDistance = playerCell.Radius;
                if (math.distancesq(playerCell.Position, food.Position) > eatDistance * eatDistance)
                {
                    continue;
                }

                if (bestEater == null || playerCell.Mass > bestEater.Mass)
                {
                    bestEater = playerCell;
                }
            }

            return bestEater;
        }

        private static void ResolvePlayerEats(this AgarRoom self)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            List<Cell> cells = new();
            foreach ((_, EntityRef<Cell> cellRef) in cellComponent.PlayerCells)
            {
                Cell cell = cellRef;
                cells.Add(cell);
            }

            List<long> eatenCellIds = new();
            for (int i = 0; i < cells.Count; ++i)
            {
                Cell a = cells[i];
                if (eatenCellIds.Contains(a.Id))
                {
                    continue;
                }

                for (int j = i + 1; j < cells.Count; ++j)
                {
                    Cell b = cells[j];
                    if (eatenCellIds.Contains(b.Id))
                    {
                        continue;
                    }

                    Cell eater = self.TryGetPlayerEater(a, b);
                    if (eater == null)
                    {
                        continue;
                    }

                    Cell victim = eater == a ? b : a;
                    eater.AddMass(victim.Mass);
                    eatenCellIds.Add(victim.Id);
                }
            }

            foreach (long cellId in eatenCellIds)
            {
                cellComponent.RemoveCell(cellId);
            }
        }

        private static Cell TryGetPlayerEater(this AgarRoom self, Cell a, Cell b)
        {
            if (a.Radius >= b.Radius * AgarRoom.MinEatRadiusRatio && self.CanEat(a, b))
            {
                return a;
            }

            if (b.Radius >= a.Radius * AgarRoom.MinEatRadiusRatio && self.CanEat(b, a))
            {
                return b;
            }

            return null;
        }

        private static bool CanEat(this AgarRoom self, Cell eater, Cell victim)
        {
            float eatDistance = eater.Radius - victim.Radius * 0.35f;
            return eatDistance > 0 && math.distancesq(eater.Position, victim.Position) <= eatDistance * eatDistance;
        }

        private static void RefillFood(this AgarRoom self)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            while (cellComponent.FoodCellIds.Count < AgarRoom.TargetFoodCount)
            {
                cellComponent.CreateCell(0, self.RandomPosition(AgarRoom.FoodRadius), AgarRoom.FoodRadius);
            }
        }

        private static void BroadcastState(this AgarRoom self)
        {
            self.LastSyncTime = TimeInfo.Instance.ServerNow();
            foreach (long playerId in self.RealPlayerIds)
            {
                self.SendToRealPlayer(playerId, self.CreateStateMessage(playerId));
            }
        }

        private static void FinishBattle(this AgarRoom self)
        {
            if (self.IsFinished)
            {
                return;
            }

            self.IsFinished = true;
            List<AgarPlayerScoreInfo> rankings = new();
            self.FillRankings(rankings);
            AgarPlayerScoreInfo winner = rankings.Count > 0 ? rankings[0] : null;

            foreach (long playerId in self.RealPlayerIds)
            {
                Match2G_AgarBattleResult result = Match2G_AgarBattleResult.Create();
                result.RoomId = self.RoomId;
                result.MyPlayerId = playerId;
                result.WinnerPlayerId = winner?.PlayerId ?? 0;
                result.WinnerScore = winner?.Score ?? 0;
                result.MyScore = self.GetPlayerScore(playerId);
                result.IsWinner = result.WinnerPlayerId == playerId;
                self.CopyRankings(rankings, result.Rankings);
                self.SendToRealPlayer(playerId, result);
            }

            self.Root().GetComponent<AgarRoomManagerComponent>().RemoveRoom(self.RoomId);
        }

        private static void FillRankings(this AgarRoom self, List<AgarPlayerScoreInfo> rankings)
        {
            rankings.Clear();
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            foreach (long playerId in self.AllPlayerIds)
            {
                AgarPlayerScoreInfo scoreInfo = AgarPlayerScoreInfo.Create();
                scoreInfo.PlayerId = playerId;
                scoreInfo.Score = self.GetPlayerScore(playerId);
                scoreInfo.Alive = cellComponent.GetPlayerCell(playerId) != null;
                rankings.Add(scoreInfo);
            }

            rankings.Sort((a, b) => b.Score.CompareTo(a.Score));
        }

        private static void CopyRankings(this AgarRoom self, List<AgarPlayerScoreInfo> from, List<AgarPlayerScoreInfo> to)
        {
            to.Clear();
            foreach (AgarPlayerScoreInfo source in from)
            {
                AgarPlayerScoreInfo scoreInfo = AgarPlayerScoreInfo.Create();
                scoreInfo.PlayerId = source.PlayerId;
                scoreInfo.Score = source.Score;
                scoreInfo.Alive = source.Alive;
                to.Add(scoreInfo);
            }
        }

        private static float GetPlayerScore(this AgarRoom self, long playerId)
        {
            Cell cell = self.GetComponent<CellComponent>().GetPlayerCell(playerId);
            return cell?.Mass ?? 0;
        }

        private static int GetAlivePlayerCount(this AgarRoom self)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            int count = 0;
            foreach (long playerId in self.AllPlayerIds)
            {
                if (cellComponent.GetPlayerCell(playerId) != null)
                {
                    ++count;
                }
            }

            return count;
        }

        private static Match2G_AgarBattleState CreateStateMessage(this AgarRoom self, long playerId)
        {
            CellComponent cellComponent = self.GetComponent<CellComponent>();
            Match2G_AgarBattleState state = Match2G_AgarBattleState.Create();
            state.RoomId = self.RoomId;
            long now = TimeInfo.Instance.ServerNow();
            state.RemainingTime = self.EndTime > now ? self.EndTime - now : 0;
            state.MyPlayerId = playerId;
            state.MyScore = self.GetPlayerScore(playerId);

            foreach ((_, EntityRef<Cell> cellRef) in cellComponent.Cells)
            {
                Cell cell = cellRef;
                AgarCellInfo cellInfo = AgarCellInfo.Create();
                cellInfo.CellId = cell.Id;
                cellInfo.OwnerPlayerId = cell.OwnerPlayerId;
                cellInfo.X = cell.Position.x;
                cellInfo.Y = cell.Position.y;
                cellInfo.Radius = cell.Radius;
                cellInfo.Mass = cell.Mass;
                state.Cells.Add(cellInfo);
            }

            self.FillRankings(state.Rankings);
            return state;
        }

        private static void SendToRealPlayer(this AgarRoom self, long playerId, IMessage message)
        {
            MessageLocationSenderComponent messageLocationSenderComponent = self.Root().GetComponent<MessageLocationSenderComponent>();
            messageLocationSenderComponent.Get(LocationType.Player).Send(playerId, message);
        }

        private static float2 RandomPosition(this AgarRoom self, float radius)
        {
            float max = AgarRoom.ArenaHalfSize - radius;
            return new float2(self.RandomRange(-max, max), self.RandomRange(-max, max));
        }

        private static float2 RandomDirection(this AgarRoom self)
        {
            float angle = self.RandomRange(0, math.PI * 2f);
            return new float2(math.cos(angle), math.sin(angle));
        }

        private static float RandomRange(this AgarRoom self, float min, float max)
        {
            return min + (max - min) * RandomGenerator.RandFloat01();
        }
    }
}
