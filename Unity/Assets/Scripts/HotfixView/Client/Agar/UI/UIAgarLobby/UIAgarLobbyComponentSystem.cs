using System;
using ET.Client;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [EntitySystemOf(typeof(UIAgarLobbyComponent))]
    [FriendOf(typeof(UIAgarLobbyComponent))]
    public static partial class UIAgarLobbyComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIAgarLobbyComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.findMatchingBtn = rc.Get<GameObject>("FindMatching");
            self.totalMatchesText = rc.Get<GameObject>("TxtTotalMatches");
            self.winMatchesText = rc.Get<GameObject>("TxtWinMatches");
            self.SetStatsLoading();
            self.findMatchingBtn.GetComponent<Button>().onClick.AddListener(() => { self.OnFindMatching().Coroutine(); });
            self.RefreshPlayerStats().Coroutine();
        }

        private static async ETTask RefreshPlayerStats(this UIAgarLobbyComponent self)
        {
            try
            {
                G2C_AgarPlayerStats response = await self.Root().GetComponent<ClientSenderComponent>().Call(C2G_AgarPlayerStats.Create()) as G2C_AgarPlayerStats;
                self.SetStatsText(response.TotalMatches, response.WinMatches);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private static void SetStatsText(this UIAgarLobbyComponent self, long totalMatches, long winMatches)
        {
            self.totalMatchesText.GetComponent<Text>().text = $"总场次：{totalMatches}";
            self.winMatchesText.GetComponent<Text>().text = $"胜利场次：{winMatches}";
        }

        private static void SetStatsLoading(this UIAgarLobbyComponent self)
        {
            self.totalMatchesText.GetComponent<Text>().text = "总场次：加载中";
            self.winMatchesText.GetComponent<Text>().text = "胜利场次：加载中";
        }

        public static async ETTask OnFindMatching(this UIAgarLobbyComponent self)
        {
            Button button = self.findMatchingBtn.GetComponent<Button>();
            button.interactable = false;

            try
            {
                await UIHelper.Create(self.Root(), UIType.UIAgarMatching, UILayer.Mid);
                await MatchingHelper.Match(self.Root());
            }
            catch (Exception e)
            {
                button.interactable = true;
                await UIHelper.Remove(self.Root(), UIType.UIAgarMatching);
                Log.Error(e);
            }
        }
    }
}
