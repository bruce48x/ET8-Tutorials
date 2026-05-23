using ET.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [EntitySystemOf(typeof(UIAgarMatchingComponent))]
    [FriendOf(typeof(UIAgarMatchingComponent))]
    public static partial class UIAgarMatchingComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIAgarMatchingComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.txtMaching = rc.Get<GameObject>("TxtMatching");
            self.StartTime = TimeInfo.Instance.ClientNow();
            self.RefreshMatchingText(0);
        }

        [EntitySystem]
        private static void Update(this UIAgarMatchingComponent self)
        {
            int second = (int)((TimeInfo.Instance.ClientNow() - self.StartTime) / 1000);
            if (second == self.LastSecond)
            {
                return;
            }

            self.RefreshMatchingText(second);
        }

        private static void RefreshMatchingText(this UIAgarMatchingComponent self, int second)
        {
            self.LastSecond = second;
            string text = $"寻找匹配中 {second} 秒";

            if (self.txtMaching.TryGetComponent(out TMP_Text tmpText))
            {
                tmpText.text = text;
                return;
            }

            if (self.txtMaching.TryGetComponent(out Text uiText))
            {
                uiText.text = text;
            }
        }
    }
}
