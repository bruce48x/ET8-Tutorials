using ET.Server.Agar;

namespace ET.Server
{
    [MessageHandler(SceneType.Match)]
    public class G2Match_AgarMatchHandler : MessageHandler<Scene, G2Match_AgarMatch, Match2G_AgarMatch>
    {
        protected override async ETTask Run(Scene scene, G2Match_AgarMatch request, Match2G_AgarMatch response)
        {
            scene.GetComponent<AgarMatchComponent>().Match(request.PlayerId, request.PlayerAccount);
            await ETTask.CompletedTask;
        }
    }
}
