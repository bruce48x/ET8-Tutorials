using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIAgarLobbyComponent : Entity, IAwake
    {
        public GameObject findMatchingBtn;
        public GameObject totalMatchesText;
        public GameObject winMatchesText;
    }
}
