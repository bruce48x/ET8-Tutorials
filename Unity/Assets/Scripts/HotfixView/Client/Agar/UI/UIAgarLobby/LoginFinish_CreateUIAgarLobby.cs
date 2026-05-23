using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Agar)]
    public class LoginFinish_CreateUIAgarLobby : AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            await UIHelper.Create(scene, UIType.UIAgarLobby, UILayer.Mid);
        }
    }
}
