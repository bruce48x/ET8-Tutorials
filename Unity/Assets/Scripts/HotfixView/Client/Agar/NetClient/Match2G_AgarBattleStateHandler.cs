using ET.Client;

namespace ET
{
    [MessageHandler(SceneType.Agar)]
    public class Match2G_AgarBattleStateHandler : MessageHandler<Scene, Match2G_AgarBattleState>
    {
        protected override async ETTask Run(Scene root, Match2G_AgarBattleState message)
        {
            UI battleUI = root.GetComponent<UIComponent>()?.Get(UIType.UIAgarBattle);
            UIAgarBattleComponent battleComponent = battleUI?.GetComponent<UIAgarBattleComponent>();
            if (battleComponent == null)
            {
                return;
            }

            battleComponent.RefreshBattleState(message);
            await ETTask.CompletedTask;
        }
    }
}
