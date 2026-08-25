// Neon Courier Transit reminder: gameplay state is explicit, compact, and independent of React.

export type GamePhase = "menu" | "run" | "paused" | "gameOver" | "shop";

export interface CharacterDefinition {
  id: number;
  name: string;
  role: string;
  price: number;
  accent: string;
  portrait: string;
}

export const CHARACTERS: CharacterDefinition[] = [
  {
    id: 0,
    name: "VANTA",
    role: "CORE COURIER",
    price: 0,
    accent: "#42E8FF",
    portrait: "/manus-storage/neorumaz-courier-vanta_17506b9c.png",
  },
  {
    id: 1,
    name: "LYRA",
    role: "VECTOR RUNNER",
    price: 1200,
    accent: "#FF4FD8",
    portrait: "/manus-storage/neorumaz-courier-lyra_eabc52b6.png",
  },
  {
    id: 2,
    name: "OREN",
    role: "SHIELD SPECIALIST",
    price: 2400,
    accent: "#FFC857",
    portrait: "/manus-storage/neorumaz-courier-oren_b5f1c92f.png",
  },
];

export interface SaveData {
  version: 1;
  coins: number;
  bestScore: number;
  selectedCharacter: number;
  unlockedCharacters: number[];
  musicEnabled: boolean;
  runsSinceInterstitial: number;
}

export interface UIState {
  phase: GamePhase;
  score: number;
  bestScore: number;
  coins: number;
  runCoins: number;
  selectedCharacter: number;
  unlockedCharacters: number[];
  musicEnabled: boolean;
  shieldSeconds: number;
  nileRushSeconds: number;
  contractProgress: number;
  contractTarget: number;
  canRevive: boolean;
  interstitialReady: boolean;
  message: string | null;
}

export interface GameBridge {
  onState: (state: UIState) => void;
}

export interface GameHandle {
  scene: import("@babylonjs/core/scene").Scene;
  start: () => void;
  pause: () => void;
  resume: () => void;
  retry: () => void;
  home: () => void;
  openShop: () => void;
  closeShop: () => void;
  selectCharacter: (id: number) => void;
  revive: () => void;
  doubleReward: () => void;
  toggleMusic: () => void;
  dispose: () => void;
}
