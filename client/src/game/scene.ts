// Neon Courier Transit reminder: Babylon owns rendering; GameWorld owns the playable rules and cleanup.

import type { Engine } from "@babylonjs/core/Engines/engine";
import { GameWorld } from "./GameWorld";
import type { GameBridge, GameHandle } from "./types";

export async function createGameScene(engine: Engine, canvas: HTMLCanvasElement, bridge: GameBridge): Promise<GameHandle> {
  const world = new GameWorld(engine, canvas, bridge);
  return world.handle;
}

export type { GameHandle } from "./types";
