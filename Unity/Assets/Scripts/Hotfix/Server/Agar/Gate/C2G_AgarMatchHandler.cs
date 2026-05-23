namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_AgarMatchHandler : MessageSessionHandler<C2G_AgarMatch, G2C_AgarMatch>
    {
        protected override async ETTask Run(Session session, C2G_AgarMatch request, G2C_AgarMatch response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;

            G2Match_AgarMatch g2MatchAgarMatch = G2Match_AgarMatch.Create();
            g2MatchAgarMatch.PlayerId = player.Id;
            await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId, g2MatchAgarMatch);
        }
    }
}
