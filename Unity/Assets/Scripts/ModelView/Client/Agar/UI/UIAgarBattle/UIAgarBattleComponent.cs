using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIAgarBattleComponent : Entity, IAwake
    {
        public GameObject txtName;
        public GameObject txtScore;
    }
}
