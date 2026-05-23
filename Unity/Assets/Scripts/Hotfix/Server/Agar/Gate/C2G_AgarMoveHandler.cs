namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_AgarMoveHandler : MessageSessionHandler<C2G_AgarMove>
    {
        protected override async ETTask Run(Session session, C2G_AgarMove message)
        {
            Player player = session.GetComponent<SessionPlayerComponent>()?.Player;
            if (player == null)
            {
                return;
            }

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Match;
            G2Match_AgarMove move = G2Match_AgarMove.Create();
            move.PlayerId = player.Id;
            move.DirectionX = message.DirectionX;
            move.DirectionY = message.DirectionY;
            session.Root().GetComponent<MessageSender>().Send(startSceneConfig.ActorId, move);
            await ETTask.CompletedTask;
        }
    }
}
