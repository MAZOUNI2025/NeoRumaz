// Neon Courier Transit reminder: persistence is safe, local, offline-first, and never rewards twice by accident.

import type { SaveData } from "./types";

const STORAGE_KEY = "neorumaz.save.v1";

const defaultSave = (): SaveData => ({
  version: 1,
  coins: 450,
  bestScore: 0,
  selectedCharacter: 0,
  unlockedCharacters: [0],
  musicEnabled: true,
  runsSinceInterstitial: 0,
});

function isValidSave(value: unknown): value is Partial<SaveData> {
  return Boolean(value && typeof value === "object");
}

export class SaveManager {
  private data: SaveData;

  constructor() {
    this.data = this.read();
  }

  get snapshot(): SaveData {
    return {
      ...this.data,
      unlockedCharacters: [...this.data.unlockedCharacters],
    };
  }

  update(partial: Partial<SaveData>) {
    this.data = {
      ...this.data,
      ...partial,
      unlockedCharacters: partial.unlockedCharacters
        ? Array.from(new Set(partial.unlockedCharacters.filter((id) => Number.isInteger(id) && id >= 0 && id <= 2)))
        : this.data.unlockedCharacters,
    };
    this.persist();
  }

  private read(): SaveData {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) return defaultSave();
      const candidate: unknown = JSON.parse(raw);
      if (!isValidSave(candidate)) return defaultSave();
      const fallback = defaultSave();
      const coins = typeof candidate.coins === "number" && candidate.coins >= 0 ? Math.floor(candidate.coins) : fallback.coins;
      const bestScore = typeof candidate.bestScore === "number" && candidate.bestScore >= 0 ? Math.floor(candidate.bestScore) : 0;
      const selectedCharacter = typeof candidate.selectedCharacter === "number" ? Math.max(0, Math.min(2, Math.floor(candidate.selectedCharacter))) : 0;
      const unlocked = Array.isArray(candidate.unlockedCharacters)
        ? Array.from(new Set(candidate.unlockedCharacters.filter((id): id is number => typeof id === "number" && Number.isInteger(id) && id >= 0 && id <= 2)))
        : [0];
      const unlockedCharacters = unlocked.includes(0) ? unlocked : [0, ...unlocked];
      return {
        version: 1,
        coins,
        bestScore,
        selectedCharacter: unlockedCharacters.includes(selectedCharacter) ? selectedCharacter : 0,
        unlockedCharacters,
        musicEnabled: typeof candidate.musicEnabled === "boolean" ? candidate.musicEnabled : true,
        runsSinceInterstitial: typeof candidate.runsSinceInterstitial === "number" ? Math.max(0, Math.floor(candidate.runsSinceInterstitial)) : 0,
      };
    } catch {
      return defaultSave();
    }
  }

  private persist() {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(this.data));
    } catch {
      // Storage failure must never prevent a playable offline run.
    }
  }
}
