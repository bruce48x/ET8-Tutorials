namespace ET.Client
{
    [MessageHandler(SceneType.Agar)]
    public class Match2G_AgarMatchSuccessHandler : MessageHandler<Scene, Match2G_AgarMatchSuccess>
    {
        protected override async ETTask Run(Scene root, Match2G_AgarMatchSuccess message)
        {
            EventSystem.Instance.Publish(root, new EnterRoom());
            await ETTask.CompletedTask;
        }
    }
}
