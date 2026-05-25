using MongoDB.Driver;

namespace ET.Server.Agar
{
    [EntitySystemOf(typeof(AgarPlayerStats))]
    [FriendOf(typeof(AgarPlayerStats))]
    [FriendOf(typeof(DBComponent))]
    public static partial class AgarPlayerStatsSystem
    {
        [EntitySystem]
        private static void Awake(this AgarPlayerStats self, string account)
        {
            self.Account = account;
        }

        public static async ETTask RecordMatchResult(this DBComponent self, string account, bool isWin)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                return;
            }

            long statsId = account.ToAgarPlayerStatsId();
            long lockKey = statsId % DBComponent.TaskCount;
            using (await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.DB, lockKey))
            {
                IMongoCollection<AgarPlayerStats> collection = self.database.GetCollection<AgarPlayerStats>(typeof(AgarPlayerStats).FullName);
                UpdateDefinition<AgarPlayerStats> update = Builders<AgarPlayerStats>.Update
                        .Set(stats => stats.Account, account)
                        .Inc(stats => stats.TotalMatches, 1)
                        .Inc(stats => stats.WinMatches, isWin ? 1 : 0)
                        .Set(stats => stats.UpdatedTime, TimeInfo.Instance.ServerNow());

                await collection.UpdateOneAsync(
                    stats => stats.Id == statsId,
                    update,
                    new UpdateOptions { IsUpsert = true });
            }
        }

        public static long ToAgarPlayerStatsId(this string account)
        {
            long hash = account.GetLongHashCode();
            if (hash == long.MinValue)
            {
                return long.MaxValue;
            }

            return hash < 0 ? -hash : hash;
        }
    }
}
