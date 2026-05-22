using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIAgarLoginComponent: Entity, IAwake
    {
        public GameObject account;
        public GameObject password;
        public GameObject loginBtn;
    }
}