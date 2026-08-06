using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Collections.Generic;

// cmd /c "C:\Users\cleme\Documents\!projects\repo-cosmetics-ui\REPOCosmetics\REPOCosmeticsDisplay\compile.bat" to compile.

namespace REPOCosmeticsDisplay
{
    [BepInPlugin("com.repo.cosmetics.display", "REPO Cosmetics Display", "1.0.0")]
    public class REPOCosmeticsMod : BaseUnityPlugin
    {
        public static REPOCosmeticsMod Instance;
        public static ManualLogSource Log;

        private readonly Harmony harmony = new Harmony("com.repo.cosmetics.display");

        public static GameObject TextInstance;
        public static TextMeshProUGUI CosmeticsText;

        private static bool        _resolved    = false;
        private static FieldInfo   _fieldRarity = null;

        private static int _rarityCommon    = -2;
        private static int _rarityUncommon  = -2;
        private static int _rarityRare      = -2;
        private static int _rarityUltraRare = -2;

        private static readonly Color ColCommon    = new Color(0.40f, 0.90f, 0.40f, 1f); // light green
        private static readonly Color ColUncommon  = new Color(0.40f, 0.75f, 1.00f, 1f); // light blue
        private static readonly Color ColRare      = new Color(0.72f, 0.40f, 1.00f, 1f); // purple
        private static readonly Color ColUltraRare = new Color(1.00f, 0.85f, 0.20f, 1f); // gold
        private static readonly Color ColDefault   = new Color(0.79f, 0.91f, 0.90f, 1f); // original teal
        private static FieldInfo   _fieldCosmeticAssets      = null;
        private static FieldInfo   _fieldCosmeticUnlocks     = null;
        private static FieldInfo   _metaManagerInstanceField = null;
        private void Awake()
        {
            Instance = this;
            Log = Logger;
            Log.LogInfo("REPO Cosmetics Display loaded");
            harmony.PatchAll();
        }

        public static void ResolveReflection()
        {
            if (_resolved) return;
            _resolved = true;

            Assembly asm = null;
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
                if (a.GetName().Name == "Assembly-CSharp") { asm = a; break; }

            if (asm == null) { Log.LogWarning("Assembly-CSharp not found"); return; }

            foreach (var t in asm.GetTypes())
            {
                if (t.Name != "MetaManager") continue;
                _fieldCosmeticAssets      = t.GetField("cosmeticAssets",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _fieldCosmeticUnlocks     = t.GetField("cosmeticUnlocks",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _metaManagerInstanceField = t.GetField("instance",
                    BindingFlags.Public | BindingFlags.Static);
                break;
            }
            Log.LogInfo("MetaManager — cosmeticAssets=" + (_fieldCosmeticAssets != null)
                + "  cosmeticUnlocks=" + (_fieldCosmeticUnlocks != null)
                + "  instance=" + (_metaManagerInstanceField != null));

            foreach (var t in asm.GetTypes())
            {
                if (t.Name != "CosmeticAsset") continue;
                _fieldRarity = t.GetField("rarity",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                Log.LogInfo("CosmeticAsset found — rarity field=" + (_fieldRarity != null));
                break;
            }

            if (_fieldRarity == null) { Log.LogWarning("rarity field not found"); return; }

            System.Type rarityType = _fieldRarity.FieldType;
            if (rarityType.IsEnum)
            {
                var names  = System.Enum.GetNames(rarityType);
                var values = System.Enum.GetValues(rarityType);
                for (int i = 0; i < names.Length; i++)
                {
                    string lower = names[i].ToLower();
                    int    val   = (int)values.GetValue(i);
                    if (lower.Contains("ultra"))         _rarityUltraRare = val;
                    else if (lower.Contains("rare"))     _rarityRare      = val;
                    else if (lower.Contains("uncommon")) _rarityUncommon  = val;
                    else if (lower.Contains("common"))   _rarityCommon    = val;
                }
                Log.LogInfo(string.Format(
                    "Rarity — Common:{0}  Uncommon:{1}  Rare:{2}  UltraRare:{3}",
                    _rarityCommon, _rarityUncommon, _rarityRare, _rarityUltraRare));
            }
        }

        public static void ReadCosmeticCounts(
            out int totalAll,       out int ownedAll,
            out int ownedCommon,    out int totalCommon,
            out int ownedUncommon,  out int totalUncommon,
            out int ownedRare,      out int totalRare,
            out int ownedUltra,     out int totalUltra)
        {
            totalAll = ownedAll = 0;
            ownedCommon = totalCommon = 0;
            ownedUncommon = totalUncommon = 0;
            ownedRare = totalRare = 0;
            ownedUltra = totalUltra = 0;

            if (_metaManagerInstanceField == null || _fieldCosmeticAssets == null || _fieldCosmeticUnlocks == null) return;

            try
            {
                object metaInstance = _metaManagerInstanceField.GetValue(null);
                if (metaInstance == null) return;

                var assetList   = _fieldCosmeticAssets.GetValue(metaInstance)  as System.Collections.IList;
                var unlockList  = _fieldCosmeticUnlocks.GetValue(metaInstance) as System.Collections.IList;
                if (assetList == null || unlockList == null) return;

                var unlockedIndices = new System.Collections.Generic.HashSet<int>();
                foreach (var idx in unlockList)
                    unlockedIndices.Add((int)idx);

                for (int i = 0; i < assetList.Count; i++)
                {
                    var asset = assetList[i];
                    if (asset == null) continue;

                    int  rarityVal = _fieldRarity != null ? (int)_fieldRarity.GetValue(asset) : -1;
                    bool isOwned   = unlockedIndices.Contains(i);

                    totalAll++;
                    if (isOwned) ownedAll++;

                    if      (rarityVal == _rarityCommon)    { totalCommon++;    if (isOwned) ownedCommon++;    }
                    else if (rarityVal == _rarityUncommon)  { totalUncommon++;  if (isOwned) ownedUncommon++;  }
                    else if (rarityVal == _rarityRare)      { totalRare++;      if (isOwned) ownedRare++;      }
                    else if (rarityVal == _rarityUltraRare) { totalUltra++;     if (isOwned) ownedUltra++;     }
                }
            }
            catch (System.Exception ex)
            {
                Log.LogDebug("CosmeticsDisplay read failed: " + ex.Message);
            }
        }

        public static string BuildHudText(
            int ownedAll,      int totalAll,
            int ownedCommon,   int totalCommon,
            int ownedUncommon, int totalUncommon,
            int ownedRare,     int totalRare,
            int ownedUltra,    int totalUltra)
        {
            string header = string.Format(
                "<color={0}>Cosmetics: {1} / {2}</color>",
                "#" + ColorUtility.ToHtmlStringRGB(ColDefault), ownedAll, totalAll);
            var parts = new List<string>();
            if (totalCommon   > 0) parts.Add(string.Format(
                "<color={0}>{1}/{2}  ●</color>", "#" + ColorUtility.ToHtmlStringRGB(ColCommon),    ownedCommon,    totalCommon));
            if (totalUncommon > 0) parts.Add(string.Format(
                "<color={0}>{1}/{2}  ●</color>", "#" + ColorUtility.ToHtmlStringRGB(ColUncommon),  ownedUncommon,  totalUncommon));
            if (totalRare     > 0) parts.Add(string.Format(
                "<color={0}>{1}/{2}  ●</color>", "#" + ColorUtility.ToHtmlStringRGB(ColRare),      ownedRare,      totalRare));
            if (totalUltra    > 0) parts.Add(string.Format(
                "<color={0}>{1}/{2}   ●</color>", "#" + ColorUtility.ToHtmlStringRGB(ColUltraRare), ownedUltra,     totalUltra));
            return parts.Count > 0
                ? header + "\n" + string.Join("\n", parts.ToArray())
                : header;
        }

        public static bool EnsureUI()
        {
            if (TextInstance != null) return true;

            GameObject hud     = GameObject.Find("Game Hud");
            GameObject taxHaul = GameObject.Find("Tax Haul");
            if (hud == null || taxHaul == null) return false;

            TMP_FontAsset font = taxHaul.GetComponent<TMP_Text>()?.font;

            TextInstance = new GameObject("CosmeticsDisplay");
            TextInstance.transform.SetParent(hud.transform, false);
            TextInstance.SetActive(false);

            CosmeticsText           = TextInstance.AddComponent<TextMeshProUGUI>();
            CosmeticsText.font      = font;
            CosmeticsText.fontSize  = 15f;
            CosmeticsText.color     = Color.white;
            CosmeticsText.alignment = TextAlignmentOptions.TopRight;
            CosmeticsText.richText  = true;

            RectTransform rt    = TextInstance.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(1f, 1f);
            rt.anchorMax        = new Vector2(1f, 1f);
            rt.pivot            = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-20f, -80f);
            rt.sizeDelta        = new Vector2(320f, 400f);

            Log.LogInfo("Cosmetics HUD text element created");
            return true;
        }
    }

    [HarmonyPatch(typeof(RoundDirector), "Update")]
    public static class RoundDirectorUpdatePatch
    {
        private static int _frameCounter = 0;
        private const  int RefreshEvery  = 120;

        private static int _cachedTotalAll      = 0, _cachedOwnedAll      = 0;
        private static int _cachedTotalCommon   = 0, _cachedOwnedCommon   = 0;
        private static int _cachedTotalUncommon = 0, _cachedOwnedUncommon = 0;
        private static int _cachedTotalRare     = 0, _cachedOwnedRare     = 0;
        private static int _cachedTotalUltra    = 0, _cachedOwnedUltra    = 0;

        [HarmonyPostfix]
        public static void Postfix()
        {
            bool mapOpen = SemiFunc.InputHold((InputKey)8)
                        || (MapToolController.instance != null
                            && Traverse.Create(MapToolController.instance)
                                       .Field("mapToggled").GetValue<bool>());

            if (!mapOpen)
            {
                if (REPOCosmeticsMod.TextInstance != null)
                    REPOCosmeticsMod.TextInstance.SetActive(false);
                return;
            }

            REPOCosmeticsMod.ResolveReflection();
            if (!REPOCosmeticsMod.EnsureUI()) return;

            _frameCounter++;
            if (_frameCounter >= RefreshEvery || _cachedTotalAll == 0)
            {
                _frameCounter = 0;
                REPOCosmeticsMod.ReadCosmeticCounts(
                    out _cachedTotalAll,      out _cachedOwnedAll,
                    out _cachedOwnedCommon,   out _cachedTotalCommon,
                    out _cachedOwnedUncommon, out _cachedTotalUncommon,
                    out _cachedOwnedRare,     out _cachedTotalRare,
                    out _cachedOwnedUltra,    out _cachedTotalUltra);
            }

            REPOCosmeticsMod.TextInstance.SetActive(true);
            REPOCosmeticsMod.CosmeticsText.SetText(
                REPOCosmeticsMod.BuildHudText(
                    _cachedOwnedAll,      _cachedTotalAll,
                    _cachedOwnedCommon,   _cachedTotalCommon,
                    _cachedOwnedUncommon, _cachedTotalUncommon,
                    _cachedOwnedRare,     _cachedTotalRare,
                    _cachedOwnedUltra,    _cachedTotalUltra));
        }
    }
}
