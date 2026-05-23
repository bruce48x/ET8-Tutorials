namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class Match2G_AgarBattleStateHandler : MessageHandler<Player, Match2G_AgarBattleState>
    {
        protected override async ETTask Run(Player player, Match2G_AgarBattleState message)
        {
            player.GetComponent<PlayerSessionComponent>().Session.Send(message);
            await ETTask.CompletedTask;
        }
    }
}
