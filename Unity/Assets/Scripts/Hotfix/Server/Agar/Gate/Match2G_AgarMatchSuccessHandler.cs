namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class Match2G_AgarMatchSuccessHandler : MessageHandler<Player, Match2G_AgarMatchSuccess>
    {
        protected override async ETTask Run(Player player, Match2G_AgarMatchSuccess message)
        {
            player.GetComponent<PlayerSessionComponent>().Session.Send(message);
            await ETTask.CompletedTask;
        }
    }
}
