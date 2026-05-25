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
            if (battleComponent == null)
            {
                return;
            }

            battleComponent.RefreshBattleResult(message);
            battleComponent.ScheduleReturnToLobby(2000);
            await ETTask.CompletedTask;
        }
    }
}
