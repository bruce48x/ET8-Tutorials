using System.Collections;
using System.Collections.Generic;
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
            self.findMatchingBtn.GetComponent<Button>().onClick.AddListener(() => { Debug.Log("点击<寻找匹配>"); });
        }
    }
}