// Cairo Night Runner style reminder: English-first premium courier progression, cyan transit hierarchy, amber reward economy and concise mobile commands.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeoRumaz.Runtime
{
    [Serializable]
    public sealed class PlayerProfileData
    {
        public int SchemaVersion = 1;
        public int Credits = 248;
        public int HighScore;
        public int TotalRuns;
        public int TotalCreditsCollected;
        public int SelectedCharacterIndex;
        public bool AudioEnabled = true;
        public List<int> UnlockedCharacterIndices = new List<int> { 0 };
    }

    public sealed class CharacterDefinition
    {
        public readonly int Index;
        public readonly string Id;
        public readonly string DisplayName;
        public readonly string Role;
        public readonly int UnlockCost;
        public readonly Color Accent;

        public CharacterDefinition(int index, string id, string displayName, string role, int unlockCost, string accentHex)
        {
            Index = index;
            Id = id;
            DisplayName = displayName;
            Role = role;
            UnlockCost = unlockCost;
            Color color;
            ColorUtility.TryParseHtmlString(accentHex, out color);
            Accent = color;
        }
    }

    public static class CharacterCatalog
    {
        private static readonly CharacterDefinition[] entries =
        {
            new CharacterDefinition(0, "nova", "NOVA", "ORIGINAL COURIER", 0, "#42E8FF"),
            new CharacterDefinition(1, "lyra", "LYRA", "NILE CIRCUIT SCOUT", 240, "#FF4FD8"),
            new CharacterDefinition(2, "oren", "OREN", "SKYLINE RUNNER", 480, "#FFC857")
        };

        public static int Count { get { return entries.Length; } }
        public static CharacterDefinition Get(int index) { return entries[Mathf.Clamp(index, 0, entries.Length - 1)]; }
    }

    public enum ShopResult
    {
        Selected,
        UnlockedAndSelected,
        AlreadySelected,
        InsufficientCredits,
        InvalidCharacter
    }

    public sealed class ProgressionService
    {
        private const string SaveKey = "neorumaz.profile.v1";
        public PlayerProfileData Profile { get; private set; }

        public void Load()
        {
            string json = PlayerPrefs.GetString(SaveKey, string.Empty);
            Profile = string.IsNullOrEmpty(json) ? new PlayerProfileData() : JsonUtility.FromJson<PlayerProfileData>(json);
            if (Profile == null) Profile = new PlayerProfileData();
            Normalize();
        }

        public void Save()
        {
            Normalize();
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(Profile));
            PlayerPrefs.Save();
        }

        public void AddCredits(int amount)
        {
            Profile.Credits = Mathf.Max(0, Profile.Credits + Mathf.Max(0, amount));
            Profile.TotalCreditsCollected += Mathf.Max(0, amount);
            Save();
        }

        public void RecordRun(int score)
        {
            Profile.TotalRuns += 1;
            Profile.HighScore = Mathf.Max(Profile.HighScore, Mathf.Max(0, score));
            Save();
        }

        public bool IsUnlocked(int index)
        {
            return Profile.UnlockedCharacterIndices.Contains(index);
        }

        public ShopResult TrySelectOrUnlock(int index)
        {
            if (index < 0 || index >= CharacterCatalog.Count) return ShopResult.InvalidCharacter;
            if (Profile.SelectedCharacterIndex == index) return ShopResult.AlreadySelected;
            if (IsUnlocked(index))
            {
                Profile.SelectedCharacterIndex = index;
                Save();
                return ShopResult.Selected;
            }
            CharacterDefinition character = CharacterCatalog.Get(index);
            if (Profile.Credits < character.UnlockCost) return ShopResult.InsufficientCredits;
            Profile.Credits -= character.UnlockCost;
            Profile.UnlockedCharacterIndices.Add(index);
            Profile.SelectedCharacterIndex = index;
            Save();
            return ShopResult.UnlockedAndSelected;
        }

        public void SetAudio(bool isEnabled)
        {
            Profile.AudioEnabled = isEnabled;
            Save();
        }

        private void Normalize()
        {
            Profile.SchemaVersion = Mathf.Max(1, Profile.SchemaVersion);
            Profile.Credits = Mathf.Max(0, Profile.Credits);
            Profile.HighScore = Mathf.Max(0, Profile.HighScore);
            Profile.TotalRuns = Mathf.Max(0, Profile.TotalRuns);
            if (Profile.UnlockedCharacterIndices == null) Profile.UnlockedCharacterIndices = new List<int>();
            if (!Profile.UnlockedCharacterIndices.Contains(0)) Profile.UnlockedCharacterIndices.Add(0);
            Profile.SelectedCharacterIndex = Mathf.Clamp(Profile.SelectedCharacterIndex, 0, CharacterCatalog.Count - 1);
            if (!Profile.UnlockedCharacterIndices.Contains(Profile.SelectedCharacterIndex)) Profile.SelectedCharacterIndex = 0;
        }
    }
}
