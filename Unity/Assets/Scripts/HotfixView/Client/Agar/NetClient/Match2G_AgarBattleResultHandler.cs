using ET.Client;

namespace ET
{
    [MessageHandler(SceneType.Agar)]
    public class Match2G_AgarBattleResultHandler : MessageHandler<Scene, Match2G_AgarBattleResult>
    {
        protected override async ETTask Run(Scene root, Match2G_AgarBattleResult message)
        {
            UI battleUI = root.GetComponent<UIComponent>()?.Get(UIType.UIAgarBattle);
            UIAgarBattleComponent battleComponent = battleUI?.GetComponent<UIAgarBattleComponent>();
            battleComponent?.RefreshBattleResult(message);

            await root.GetComponent<TimerComponent>().WaitAsync(2000);

            await UIHelper.Remove(root, UIType.UIAgarBattle);
            await UIHelper.Remove(root, UIType.UIAgarMatching);
            await UIHelper.Remove(root, UIType.UIAgarLobby);
            await UIHelper.Create(root, UIType.UIAgarLobby, UILayer.Mid);
        }
    }
}
