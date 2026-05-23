using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIAgarLoginComponent))]
    [FriendOf(typeof(UIAgarLoginComponent))]
    public static partial class UIAgarLoginComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIAgarLoginComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.loginBtn = rc.Get<GameObject>("LoginBtn");
            self.password = rc.Get<GameObject>("Password");
            self.account = rc.Get<GameObject>("Account");

            self.loginBtn.GetComponent<Button>().onClick.AddListener(() => { self.OnLogin(); });
        }

        public static void OnLogin(this UIAgarLoginComponent self)
        {
            LoginHelper.Login(self.Root(), self.account.GetComponent<InputField>().text, self.password.GetComponent<InputField>().text).Coroutine();
        }
    }
}
