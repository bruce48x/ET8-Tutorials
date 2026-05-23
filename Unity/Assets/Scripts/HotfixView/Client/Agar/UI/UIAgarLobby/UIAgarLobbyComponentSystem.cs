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
            self.findMatchingBtn.GetComponent<Button>().onClick.AddListener(() => { self.OnFindMatching().Coroutine(); });
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
