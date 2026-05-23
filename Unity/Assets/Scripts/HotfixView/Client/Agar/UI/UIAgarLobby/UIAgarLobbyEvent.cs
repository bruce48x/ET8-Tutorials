using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [UIEvent(UIType.UIAgarLobby)]
    public class UIAgarLobbyEvent : AUIEvent
    {
        public override async ETTask<UI> OnCreate(UIComponent uiComponent, UILayer uiLayer)
        {
            string assetsName = $"Assets/Bundles/UI/Agar/{UIType.UIAgarLobby}.prefab";
            GameObject bundleGameObject = await uiComponent.Scene().GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<GameObject>(assetsName);
            GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject, uiComponent.UIGlobalComponent.GetLayer((int)uiLayer));
            UI ui = uiComponent.AddChild<UI, string, GameObject>(UIType.UIAgarLobby, gameObject);
            ui.AddComponent<UIAgarLobbyComponent>();
            return ui;
        }
        
        public override void OnRemove(UIComponent uiComponent)
        {
        }
    }
}
