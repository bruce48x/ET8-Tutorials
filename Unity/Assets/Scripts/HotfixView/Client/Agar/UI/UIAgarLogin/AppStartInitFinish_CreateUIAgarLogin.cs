using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Agar)]
    public class AppStartInitFinish_CreateUIAgarLogin : AEvent<Scene, AppStartInitFinish>
    {
        protected override async ETTask Run(Scene scene, AppStartInitFinish args)
        {
            await UIHelper.Create(scene, UIType.UIAgarLogin, UILayer.Mid);
        }
    }
}
