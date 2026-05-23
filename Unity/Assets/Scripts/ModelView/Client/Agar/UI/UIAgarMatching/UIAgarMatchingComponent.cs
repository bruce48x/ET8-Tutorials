using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIAgarMatchingComponent : Entity, IAwake, IUpdate
    {
        public GameObject txtMaching;
        public long StartTime;
        public int LastSecond = -1;
    }
}
