using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIAgarLobbyComponent : Entity, IAwake
    {
        public GameObject findMatchingBtn;
    }
}
