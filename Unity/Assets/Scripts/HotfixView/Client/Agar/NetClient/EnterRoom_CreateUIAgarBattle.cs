using ET.Client;

namespace ET
{
    [Event(SceneType.Agar)]
    public class EnterRoom_CreateUIAgarBattle : AEvent<Scene, EnterRoom>
    {
        protected override async ETTask Run(Scene scene, EnterRoom args)
        {
            await UIHelper.Remove(scene, UIType.UIAgarMatching);
            await UIHelper.Remove(scene, UIType.UIAgarLobby);
            UI battleUI = await UIHelper.Create(scene, UIType.UIAgarBattle, UILayer.Mid);
            battleUI.GetComponent<UIAgarBattleComponent>()?.InitializeCountdown(args.EndTime);
        }
    }
}
