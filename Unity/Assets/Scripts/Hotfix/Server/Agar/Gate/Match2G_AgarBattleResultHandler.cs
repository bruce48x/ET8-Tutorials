namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class Match2G_AgarBattleResultHandler : MessageHandler<Player, Match2G_AgarBattleResult>
    {
        protected override async ETTask Run(Player player, Match2G_AgarBattleResult message)
        {
            player.GetComponent<PlayerSessionComponent>().Session.Send(message);
            await ETTask.CompletedTask;
        }
    }
}
