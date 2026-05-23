using ET.Server.Agar;

namespace ET.Server
{
    [MessageHandler(SceneType.Match)]
    public class G2Match_AgarMoveHandler : MessageHandler<Scene, G2Match_AgarMove>
    {
        protected override async ETTask Run(Scene scene, G2Match_AgarMove message)
        {
            AgarRoom room = scene.GetComponent<AgarRoomManagerComponent>().GetRoomByPlayer(message.PlayerId);
            room?.SetPlayerDirection(message.PlayerId, message.DirectionX, message.DirectionY);
            await ETTask.CompletedTask;
        }
    }
}
