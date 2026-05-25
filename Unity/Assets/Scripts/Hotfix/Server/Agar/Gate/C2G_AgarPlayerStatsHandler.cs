using ET.Server.Agar;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_AgarPlayerStatsHandler : MessageSessionHandler<C2G_AgarPlayerStats, G2C_AgarPlayerStats>
    {
        protected override async ETTask Run(Session session, C2G_AgarPlayerStats request, G2C_AgarPlayerStats response)
        {
            SessionPlayerComponent sessionPlayerComponent = session.GetComponent<SessionPlayerComponent>();
            if (sessionPlayerComponent == null || sessionPlayerComponent.Player == null)
            {
                response.Error = ErrorCore.ERR_NotFoundActor;
                response.Message = "Player not found";
                return;
            }

            Player player = sessionPlayerComponent.Player;
            DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
            AgarPlayerStats stats = await dbComponent.Query<AgarPlayerStats>(player.Account.ToAgarPlayerStatsId());
            if (stats == null)
            {
                return;
            }

            response.TotalMatches = stats.TotalMatches;
            response.WinMatches = stats.WinMatches;
        }
    }
}
