using System.Collections.Generic;
using ET.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [EntitySystemOf(typeof(UIAgarBattleComponent))]
    [FriendOf(typeof(UIAgarBattleComponent))]
    public static partial class UIAgarBattleComponentSystem
    {
        private const float ArenaHalfSize = 80f;

        [EntitySystem]
        private static void Awake(this UIAgarBattleComponent self)
        {
            GameObject root = self.GetParent<UI>().GameObject;
            ReferenceCollector rc = root.GetComponent<ReferenceCollector>();
            self.txtName = self.GetReferenceOrChild(rc, root, "TxtName");
            self.txtScore = self.GetReferenceOrChild(rc, root, "TxtScore");
            self.txtTime = self.GetReferenceOrChild(rc, root, "TxtTime");
            self.txtResult = self.GetReferenceOrChild(rc, root, "TxtResult");
            self.circleSprite = self.CreateCircleSprite();
            self.CreateArena(root);
            self.CreateHudTextIfMissing(root);
            self.RefreshBattleState(0, 0);
            self.SetText(self.txtResult, string.Empty);
        }

        [EntitySystem]
        private static void Destroy(this UIAgarBattleComponent self)
        {
            self.CellViews.Clear();
            self.arenaRoot = null;
            if (self.circleSprite != null)
            {
                UnityEngine.Object.Destroy(self.circleSprite.texture);
                UnityEngine.Object.Destroy(self.circleSprite);
            }

            self.circleSprite = null;
        }

        [EntitySystem]
        private static void Update(this UIAgarBattleComponent self)
        {
            self.UpdateCountdown();
            self.UpdateMoveInput();
        }

        public static void InitializeCountdown(this UIAgarBattleComponent self, long endTime)
        {
            self.BattleEndTime = endTime;
            self.IsBattleFinished = false;
            self.ReturnToLobbyScheduled = false;
            self.UpdateCountdown();
        }

        public static void RefreshBattleState(this UIAgarBattleComponent self, Match2G_AgarBattleState state)
        {
            self.CurrentScore = state.MyScore;
            self.BattleEndTime = TimeInfo.Instance.ServerNow() + state.RemainingTime;
            self.IsBattleFinished = false;
            self.RefreshBattleState(state.RemainingTime, state.MyScore);
            self.RefreshCells(state);
        }

        public static void RefreshBattleState(this UIAgarBattleComponent self, long remainingTime, float score)
        {
            self.CurrentScore = score;
            self.SetText(self.txtName, "Agar Battle");
            self.SetText(self.txtScore, $"Score {score:0}");
            self.SetCountdownText(remainingTime);
        }

        public static void RefreshBattleResult(this UIAgarBattleComponent self, Match2G_AgarBattleResult result)
        {
            self.IsBattleFinished = true;
            string resultText = result.IsWinner ? "Victory" : $"Defeat Winner {result.WinnerPlayerId}";
            self.SetText(self.txtResult, resultText);
            self.SetText(self.txtScore, $"Score {result.MyScore:0}");
            self.SetCountdownText(0);
        }

        private static void UpdateCountdown(this UIAgarBattleComponent self)
        {
            if (self.IsBattleFinished || self.BattleEndTime <= 0)
            {
                return;
            }

            long remainingTime = self.BattleEndTime - TimeInfo.Instance.ServerNow();
            self.SetCountdownText(remainingTime);
            if (remainingTime <= 0)
            {
                self.IsBattleFinished = true;
                self.ScheduleReturnToLobby(2000);
            }
        }

        private static void SetCountdownText(this UIAgarBattleComponent self, long remainingTime)
        {
            long seconds = remainingTime > 0 ? (remainingTime + 999) / 1000 : 0;
            self.SetText(self.txtTime, $"倒计时 {seconds}s");
        }

        private static void UpdateMoveInput(this UIAgarBattleComponent self)
        {
            if (self.IsBattleFinished)
            {
                return;
            }

            float x = 0;
            float y = 0;

            if (Input.GetKey(KeyCode.A))
            {
                x -= 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                x += 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                y -= 1;
            }

            if (Input.GetKey(KeyCode.W))
            {
                y += 1;
            }

            Vector2 direction = new(x, y);
            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
            }

            long now = TimeInfo.Instance.ClientNow();
            bool changed = Mathf.Abs(direction.x - self.LastMoveDirectionX) > 0.01f ||
                    Mathf.Abs(direction.y - self.LastMoveDirectionY) > 0.01f;
            bool heartbeat = now - self.LastMoveSendTime >= 150;
            if (!changed && !heartbeat)
            {
                return;
            }

            self.LastMoveSendTime = now;
            self.LastMoveDirectionX = direction.x;
            self.LastMoveDirectionY = direction.y;

            C2G_AgarMove move = C2G_AgarMove.Create();
            move.DirectionX = direction.x;
            move.DirectionY = direction.y;
            self.Root().GetComponent<ClientSenderComponent>()?.Send(move);
        }

        private static void RefreshCells(this UIAgarBattleComponent self, Match2G_AgarBattleState state)
        {
            if (self.arenaRoot == null)
            {
                return;
            }

            HashSet<long> visibleCellIds = new();
            foreach (AgarCellInfo cellInfo in state.Cells)
            {
                visibleCellIds.Add(cellInfo.CellId);
                GameObject cellView = self.GetOrCreateCellView(cellInfo);
                self.ApplyCellView(cellView, cellInfo, state.MyPlayerId);
            }

            List<long> staleCellIds = new();
            foreach ((long cellId, _) in self.CellViews)
            {
                if (!visibleCellIds.Contains(cellId))
                {
                    staleCellIds.Add(cellId);
                }
            }

            foreach (long cellId in staleCellIds)
            {
                UnityEngine.Object.Destroy(self.CellViews[cellId]);
                self.CellViews.Remove(cellId);
            }
        }

        private static GameObject GetOrCreateCellView(this UIAgarBattleComponent self, AgarCellInfo cellInfo)
        {
            if (self.CellViews.TryGetValue(cellInfo.CellId, out GameObject cellView))
            {
                return cellView;
            }

            cellView = new GameObject(cellInfo.OwnerPlayerId == 0 ? $"Food_{cellInfo.CellId}" : $"PlayerCell_{cellInfo.OwnerPlayerId}");
            cellView.layer = self.arenaRoot.gameObject.layer;
            RectTransform rectTransform = cellView.AddComponent<RectTransform>();
            rectTransform.SetParent(self.arenaRoot, false);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            Image image = cellView.AddComponent<Image>();
            image.sprite = self.circleSprite;
            image.raycastTarget = false;

            self.CellViews.Add(cellInfo.CellId, cellView);
            return cellView;
        }

        private static void ApplyCellView(this UIAgarBattleComponent self, GameObject cellView, AgarCellInfo cellInfo, long myPlayerId)
        {
            RectTransform rectTransform = cellView.GetComponent<RectTransform>();
            float scale = self.GetArenaScale();
            rectTransform.anchoredPosition = new Vector2(cellInfo.X * scale, cellInfo.Y * scale);
            float diameter = Mathf.Max(4f, cellInfo.Radius * 2f * scale);
            rectTransform.sizeDelta = new Vector2(diameter, diameter);

            Image image = cellView.GetComponent<Image>();
            image.color = self.GetCellColor(cellInfo.OwnerPlayerId, myPlayerId);
            cellView.transform.SetAsLastSibling();
        }

        private static float GetArenaScale(this UIAgarBattleComponent self)
        {
            Rect rect = self.arenaRoot.rect;
            float width = rect.width > 0 ? rect.width : 1280f;
            float height = rect.height > 0 ? rect.height : 720f;
            return Mathf.Min(width, height) / (ArenaHalfSize * 2f);
        }

        private static Color GetCellColor(this UIAgarBattleComponent self, long ownerPlayerId, long myPlayerId)
        {
            if (ownerPlayerId == 0)
            {
                return new Color(0.35f, 0.9f, 0.45f, 0.95f);
            }

            if (ownerPlayerId == myPlayerId)
            {
                return new Color(0.15f, 0.65f, 1f, 0.95f);
            }

            int hash = Mathf.Abs(ownerPlayerId.GetHashCode());
            float hue = (hash % 360) / 360f;
            return Color.HSVToRGB(hue, 0.72f, 0.95f);
        }

        private static void CreateArena(this UIAgarBattleComponent self, GameObject root)
        {
            Transform parent = root.transform.Find("Panel") ?? root.transform;
            GameObject arena = new("AgarArena");
            arena.layer = root.layer;
            RectTransform rectTransform = arena.AddComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.SetAsFirstSibling();

            Image background = arena.AddComponent<Image>();
            background.color = new Color(0.05f, 0.08f, 0.1f, 0.86f);
            background.raycastTarget = false;

            self.arenaRoot = rectTransform;
        }

        private static void CreateHudTextIfMissing(this UIAgarBattleComponent self, GameObject root)
        {
            Transform parent = root.transform.Find("Panel") ?? root.transform;
            self.txtTime ??= self.CreateHudText(parent, "TxtTime", new Vector2(0.5f, 1f), new Vector2(0f, -36f), TextAnchor.UpperCenter);
            self.txtResult ??= self.CreateHudText(parent, "TxtResult", new Vector2(0.5f, 0.5f), Vector2.zero, TextAnchor.MiddleCenter);
        }

        private static GameObject CreateHudText(this UIAgarBattleComponent self, Transform parent, string name, Vector2 anchor, Vector2 position, TextAnchor alignment)
        {
            GameObject textObject = new(name);
            textObject.layer = parent.gameObject.layer;
            RectTransform rectTransform = textObject.AddComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(420f, 64f);

            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = name == "TxtResult" ? 40 : 24;
            text.alignment = alignment;
            text.color = Color.white;
            text.raycastTarget = false;
            return textObject;
        }

        private static Sprite CreateCircleSprite(this UIAgarBattleComponent self)
        {
            const int size = 64;
            Texture2D texture = new(size, size, TextureFormat.RGBA32, false);
            texture.name = "AgarRuntimeCircle";
            texture.wrapMode = TextureWrapMode.Clamp;

            Vector2 center = new((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.48f;
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01(radius - distance + 1f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private static GameObject GetReferenceOrChild(this UIAgarBattleComponent self, ReferenceCollector rc, GameObject root, string key)
        {
            GameObject reference = rc?.Get<GameObject>(key);
            if (reference != null)
            {
                return reference;
            }

            Transform child = self.FindChild(root.transform, key);
            return child?.gameObject;
        }

        private static Transform FindChild(this UIAgarBattleComponent self, Transform parent, string name)
        {
            if (parent.name == name)
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform result = self.FindChild(parent.GetChild(i), name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static void SetText(this UIAgarBattleComponent self, GameObject gameObject, string text)
        {
            if (gameObject == null)
            {
                return;
            }

            if (gameObject.TryGetComponent(out TMP_Text tmpText))
            {
                tmpText.text = text;
                return;
            }

            if (gameObject.TryGetComponent(out Text uiText))
            {
                uiText.text = text;
            }
        }

        public static void ScheduleReturnToLobby(this UIAgarBattleComponent self, long delayMilliseconds)
        {
            if (self.ReturnToLobbyScheduled)
            {
                return;
            }

            self.ReturnToLobbyScheduled = true;
            self.ReturnToLobbyAsync(delayMilliseconds).Coroutine();
        }

        private static async ETTask ReturnToLobbyAsync(this UIAgarBattleComponent self, long delayMilliseconds)
        {
            Scene root = self.Root();
            await root.GetComponent<TimerComponent>().WaitAsync(delayMilliseconds);
            await UIHelper.Remove(root, UIType.UIAgarBattle);
            await UIHelper.Remove(root, UIType.UIAgarMatching);
            await UIHelper.Remove(root, UIType.UIAgarLobby);
            await UIHelper.Create(root, UIType.UIAgarLobby, UILayer.Mid);
        }
    }
}
