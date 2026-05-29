using System;
using System.Collections.Generic;
using UnityEngine;

namespace REPOCosmeticsDisplay
{
    public enum CosmeticRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public class Cosmetic
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public CosmeticRarity Rarity { get; set; }
        public bool IsUnlocked { get; set; }
        public Texture2D Icon { get; set; }
        public Color GetRarityColor()
        {
            switch (Rarity)
            {
                case CosmeticRarity.Common:
                    return Color.white;
                case CosmeticRarity.Uncommon:
                    return Color.green;
                case CosmeticRarity.Rare:
                    return Color.blue;
                case CosmeticRarity.Legendary:
                    return new Color(1f, 0.85f, 0f); // Gold
                default:
                    return Color.white;
            }
        }

        public string GetRarityText()
        {
            switch (Rarity)
            {
                case CosmeticRarity.Common:
                    return "Common";
                case CosmeticRarity.Uncommon:
                    return "Uncommon";
                case CosmeticRarity.Rare:
                    return "Rare";
                case CosmeticRarity.Legendary:
                    return "Legendary";
                default:
                    return "Unknown";
            }
        }
    }

    public class CosmeticsManager
    {
        private Dictionary<CosmeticRarity, List<Cosmetic>> cosmeticsByRarity;
        private List<Cosmetic> allCosmetics;

        public CosmeticsManager()
        {
            cosmeticsByRarity = new Dictionary<CosmeticRarity, List<Cosmetic>>
            {
                { CosmeticRarity.Common, new List<Cosmetic>() },
                { CosmeticRarity.Uncommon, new List<Cosmetic>() },
                { CosmeticRarity.Rare, new List<Cosmetic>() },
                { CosmeticRarity.Legendary, new List<Cosmetic>() }
            };
            allCosmetics = new List<Cosmetic>();
            
            InitializeCosmetics();
        }

        private void InitializeCosmetics()
        {
            AddCosmetic(new Cosmetic
            {
                ID = "common_01",
                Name = "Basic Red",
                Rarity = CosmeticRarity.Common,
                IsUnlocked = true
            });
            
            AddCosmetic(new Cosmetic
            {
                ID = "common_02",
                Name = "Basic Blue",
                Rarity = CosmeticRarity.Common,
                IsUnlocked = true
            });
            
            AddCosmetic(new Cosmetic
            {
                ID = "uncommon_01",
                Name = "Shadow Camo",
                Rarity = CosmeticRarity.Uncommon,
                IsUnlocked = true
            });
            
            AddCosmetic(new Cosmetic
            {
                ID = "uncommon_02",
                Name = "Digital Camo",
                Rarity = CosmeticRarity.Uncommon,
                IsUnlocked = false
            });
            
            AddCosmetic(new Cosmetic
            {
                ID = "rare_01",
                Name = "Neon Spirit",
                Rarity = CosmeticRarity.Rare,
                IsUnlocked = true
            });
            
            AddCosmetic(new Cosmetic
            {
                ID = "rare_02",
                Name = "Arctic Frost",
                Rarity = CosmeticRarity.Rare,
                IsUnlocked = false
            });
            
            AddCosmetic(new Cosmetic
            {
                ID = "legendary_01",
                Name = "Golden Titan",
                Rarity = CosmeticRarity.Legendary,
                IsUnlocked = false
            });
            
            AddCosmetic(new Cosmetic
            {
                ID = "legendary_02",
                Name = "Void Master",
                Rarity = CosmeticRarity.Legendary,
                IsUnlocked = false
            });
        }

        public void AddCosmetic(Cosmetic cosmetic)
        {
            allCosmetics.Add(cosmetic);
            cosmeticsByRarity[cosmetic.Rarity].Add(cosmetic);
        }

        public List<Cosmetic> GetCosmeticsByRarity(CosmeticRarity rarity)
        {
            return new List<Cosmetic>(cosmeticsByRarity[rarity]);
        }

        public List<Cosmetic> GetUnlockedCosmetics()
        {
            List<Cosmetic> unlocked = new List<Cosmetic>();
            foreach (var cosmetic in allCosmetics)
            {
                if (cosmetic.IsUnlocked)
                    unlocked.Add(cosmetic);
            }
            return unlocked;
        }

        public List<Cosmetic> GetUnlockedCosmeticsByRarity(CosmeticRarity rarity)
        {
            List<Cosmetic> unlocked = new List<Cosmetic>();
            foreach (var cosmetic in cosmeticsByRarity[rarity])
            {
                if (cosmetic.IsUnlocked)
                    unlocked.Add(cosmetic);
            }
            return unlocked;
        }

        public int GetTotalUnlockedCount()
        {
            return GetUnlockedCosmetics().Count;
        }

        public int GetTotalCount()
        {
            return allCosmetics.Count;
        }

        public Cosmetic GetCosmeticByID(string id)
        {
            return allCosmetics.Find(c => c.ID == id);
        }

        public void UnlockCosmetic(string id)
        {
            var cosmetic = GetCosmeticByID(id);
            if (cosmetic != null)
                cosmetic.IsUnlocked = true;
        }

        public void LockCosmetic(string id)
        {
            var cosmetic = GetCosmeticByID(id);
            if (cosmetic != null)
                cosmetic.IsUnlocked = false;
        }
    }
}
