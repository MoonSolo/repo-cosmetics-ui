using UnityEngine;
using System.Collections.Generic;

namespace REPOCosmeticsDisplay
{
    public class UIManager
    {
        private CosmeticsManager cosmeticsManager;
        private bool isVisible = false;

        private Rect windowRect;
        private bool windowRectInitialized = false;

        private Vector2 scrollPosition = Vector2.zero;

        private bool stylesInitialized = false;
        private GUIStyle headerStyle;
        private GUIStyle rarityStyle;

        private const float WINDOW_WIDTH  = 340f;
        private const float WINDOW_HEIGHT = 500f;
        private const float ITEM_HEIGHT   = 50f;
        private const float HEADER_HEIGHT = 30f;

        public UIManager(CosmeticsManager manager)
        {
            cosmeticsManager = manager;
        }

        private void InitializeStyles()
        {
            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = Color.white }
            };

            rarityStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding   = new RectOffset(5, 5, 3, 3)
            };
        }

        public void Toggle()        => isVisible = !isVisible;
        public void SetVisible(bool visible) => isVisible = visible;
        public bool IsVisible => isVisible;

        public void DrawUI()
        {
            if (!isVisible)
                return;

            if (!stylesInitialized)
            {
                InitializeStyles();
                stylesInitialized = true;
            }

            if (!windowRectInitialized)
            {
                windowRect = new Rect(Screen.width - WINDOW_WIDTH - 10f, 100f, WINDOW_WIDTH, WINDOW_HEIGHT);
                windowRectInitialized = true;
            }

            try
            {
                windowRect = GUILayout.Window(9842, windowRect, DrawCosmeticsWindow,
                    "COSMETICS", GUILayout.Width(WINDOW_WIDTH), GUILayout.Height(WINDOW_HEIGHT));
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[REPOCosmetics] Error drawing UI: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DrawCosmeticsWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.Label(
                string.Format("Unlocked: {0} / {1}",
                    cosmeticsManager.GetTotalUnlockedCount(),
                    cosmeticsManager.GetTotalCount()),
                headerStyle,
                GUILayout.Height(HEADER_HEIGHT));

            GUILayout.Space(4f);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            DrawCosmeticsByRarity(CosmeticRarity.Common);
            DrawCosmeticsByRarity(CosmeticRarity.Uncommon);
            DrawCosmeticsByRarity(CosmeticRarity.Rare);
            DrawCosmeticsByRarity(CosmeticRarity.Legendary);

            GUILayout.EndScrollView();

            GUILayout.Space(4f);
            GUILayout.Label("Hold Tab to view | Release to hide",
                new GUIStyle(GUI.skin.label) { fontSize = 10, alignment = TextAnchor.MiddleCenter });

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }

        private void DrawCosmeticsByRarity(CosmeticRarity rarity)
        {
            var cosmetics = cosmeticsManager.GetCosmeticsByRarity(rarity);
            if (cosmetics.Count == 0)
                return;

            int unlockedCount = cosmeticsManager.GetUnlockedCosmeticsByRarity(rarity).Count;
            Color rarityColor = cosmetics[0].GetRarityColor();

            GUI.color = rarityColor;
            GUILayout.Label(
                string.Format("{0}  ({1}/{2})", cosmetics[0].GetRarityText(), unlockedCount, cosmetics.Count),
                rarityStyle,
                GUILayout.Height(22f));
            GUI.color = Color.white;

            foreach (var cosmetic in cosmetics)
                DrawCosmeticItem(cosmetic);

            GUILayout.Space(8f);
        }

        private void DrawCosmeticItem(Cosmetic cosmetic)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(ITEM_HEIGHT));

            if (cosmetic.IsUnlocked)
            {
                GUI.color = cosmetic.GetRarityColor();
                GUILayout.Label("\u2713", new GUIStyle(GUI.skin.label) { fontSize = 18 }, GUILayout.Width(22f));
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = new Color(0.4f, 0.4f, 0.4f);
                GUILayout.Label("\u2717", new GUIStyle(GUI.skin.label) { fontSize = 18 }, GUILayout.Width(22f));
                GUI.color = Color.white;
            }

            GUILayout.BeginVertical();
            Color nameColor = cosmetic.IsUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label(cosmetic.Name,
                new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, normal = { textColor = nameColor } });
            GUILayout.Label("ID: " + cosmetic.ID,
                new GUIStyle(GUI.skin.label) { fontSize = 9, normal = { textColor = new Color(0.6f, 0.6f, 0.6f) } });
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }
}
