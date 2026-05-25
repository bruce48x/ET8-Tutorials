using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIAgarBattleComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public GameObject txtName;
        public GameObject txtScore;
        public GameObject txtTime;
        public GameObject txtResult;
        public RectTransform arenaRoot;
        public Sprite circleSprite;
        public long LastMoveSendTime;
        public float LastMoveDirectionX;
        public float LastMoveDirectionY;
        public long BattleEndTime;
        public float CurrentScore;
        public bool IsBattleFinished;
        public bool ReturnToLobbyScheduled;
        public readonly Dictionary<long, GameObject> CellViews = new();
    }
}
