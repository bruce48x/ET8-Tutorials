using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Agar)]
    public class LoginFinish_RemoveUIAgarLogin : AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            await UIHelper.Remove(scene, UIType.UIAgarLogin);
        }
    }
}
