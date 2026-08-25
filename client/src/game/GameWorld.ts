// Neon Courier Transit reminder: center-screen readability, fair three-lane decisions, restrained glow, no React coupling.

import { Vector3 } from "@babylonjs/core/Maths/math.vector";
import { Color3, Color4 } from "@babylonjs/core/Maths/math.color";
import { ArcRotateCamera } from "@babylonjs/core/Cameras/arcRotateCamera";
import { HemisphericLight } from "@babylonjs/core/Lights/hemisphericLight";
import { PointLight } from "@babylonjs/core/Lights/pointLight";
import { MeshBuilder } from "@babylonjs/core/Meshes/meshBuilder";
import { Mesh } from "@babylonjs/core/Meshes/mesh";
import { TransformNode } from "@babylonjs/core/Meshes/transformNode";
import { StandardMaterial } from "@babylonjs/core/Materials/standardMaterial";
import { Scene } from "@babylonjs/core/scene";
import type { Engine } from "@babylonjs/core/Engines/engine";
import { AudioManager } from "./AudioManager";
import { SaveManager } from "./SaveManager";
import { CHARACTERS, type GameBridge, type GameHandle, type GamePhase, type UIState } from "./types";

type ObstacleKind = "barrier" | "drone";

interface MovingEntity {
  root: TransformNode;
  lane: number;
  active: boolean;
  kind?: ObstacleKind;
  spin?: number;
}

const LANES = [-3.1, 0, 3.1];
const ROAD_SEGMENT_LENGTH = 20;
const ROAD_SEGMENTS = 8;

export class GameWorld implements Omit<GameHandle, "scene" | "dispose"> {
  private readonly scene: Scene;
  private readonly bridge: GameBridge;
  private readonly save = new SaveManager();
  private readonly audio: AudioManager;
  private phase: GamePhase = "menu";
  private player!: TransformNode;
  private playerBody!: Mesh;
  private playerTrail!: Mesh;
  private shieldRing!: Mesh;
  private camera!: ArcRotateCamera;
  private roadSegments: TransformNode[] = [];
  private cityBlocks: TransformNode[] = [];
  private obstacles: MovingEntity[] = [];
  private coins: MovingEntity[] = [];
  private shields: MovingEntity[] = [];
  private inputStart: { x: number; y: number } | null = null;
  private laneIndex = 1;
  private playerY = 0;
  private verticalVelocity = 0;
  private shieldSeconds = 0;
  private scoreDistance = 0;
  private runCoins = 0;
  private speed = 15;
  private spawnTimer = 0.7;
  private uiTimer = 0;
  private revived = false;
  private doubleClaimed = false;
  private message: string | null = null;
  private interstitialReady = false;
  private demo = new URLSearchParams(window.location.search).has("demo");
  private demoTimer = 0;
  private disposed = false;

  constructor(engine: Engine, canvas: HTMLCanvasElement, bridge: GameBridge) {
    this.scene = new Scene(engine);
    this.bridge = bridge;
    this.audio = new AudioManager(this.save.snapshot.musicEnabled);
    this.player = new TransformNode("player-root", this.scene);
    this.createEnvironment();
    this.createPlayer();
    this.createPools();
    this.bindInput(canvas);
    this.scene.onBeforeRenderObservable.add(() => this.update());
    this.emitState();
    if (this.demo) window.setTimeout(() => this.start(), 350);
  }

  get handle(): GameHandle {
    return {
      scene: this.scene,
      start: this.start,
      pause: this.pause,
      resume: this.resume,
      retry: this.retry,
      home: this.home,
      openShop: this.openShop,
      closeShop: this.closeShop,
      selectCharacter: this.selectCharacter,
      revive: this.revive,
      doubleReward: this.doubleReward,
      toggleMusic: this.toggleMusic,
      dispose: this.dispose,
    };
  }

  private makeMaterial(name: string, diffuse: string, emission = "#000000", alpha = 1) {
    const material = new StandardMaterial(name, this.scene);
    material.diffuseColor = Color3.FromHexString(diffuse);
    material.emissiveColor = Color3.FromHexString(emission);
    material.specularColor = new Color3(0.08, 0.08, 0.1);
    material.alpha = alpha;
    return material;
  }

  private createEnvironment() {
    this.scene.clearColor = Color4.FromHexString("#050814FF");
    this.scene.ambientColor = Color3.FromHexString("#0B1830");
    this.camera = new ArcRotateCamera("runner-camera", -Math.PI / 2, 1.08, 18, new Vector3(0, 0.55, 8), this.scene);
    this.camera.lowerRadiusLimit = 18;
    this.camera.upperRadiusLimit = 18;
    this.camera.lowerBetaLimit = 1.08;
    this.camera.upperBetaLimit = 1.08;
    this.camera.inputs.clear();
    const skyLight = new HemisphericLight("sky-light", new Vector3(0, 1, 0), this.scene);
    skyLight.diffuse = Color3.FromHexString("#9BD8FF");
    skyLight.groundColor = Color3.FromHexString("#07101F");
    skyLight.intensity = 0.84;
    const cyanLight = new PointLight("cyan-light", new Vector3(0, 4, 8), this.scene);
    cyanLight.diffuse = Color3.FromHexString("#42E8FF");
    cyanLight.intensity = 2.1;
    cyanLight.range = 26;
    const horizonLight = new PointLight("horizon-light", new Vector3(0, 9, 78), this.scene);
    horizonLight.diffuse = Color3.FromHexString("#FF4FD8");
    horizonLight.intensity = 1.6;
    horizonLight.range = 90;

    const roadMat = this.makeMaterial("road-mat", "#141B2B", "#050912");
    const edgeMat = this.makeMaterial("edge-mat", "#1D2735", "#0D2434");
    const laneMat = this.makeMaterial("lane-mat", "#173A48", "#42E8FF");
    const railMat = this.makeMaterial("rail-mat", "#253144", "#132C42");
    const magentaMat = this.makeMaterial("billboard-mat", "#23122D", "#FF4FD8");

    for (let i = 0; i < ROAD_SEGMENTS; i += 1) {
      const segment = new TransformNode(`road-${i}`, this.scene);
      segment.position.z = -20 + i * ROAD_SEGMENT_LENGTH;
      const road = MeshBuilder.CreateBox(`road-base-${i}`, { width: 12.8, height: 0.22, depth: ROAD_SEGMENT_LENGTH }, this.scene);
      road.parent = segment;
      road.position.y = -0.18;
      road.material = roadMat;
      for (const x of [-2.05, 2.05]) {
        const lane = MeshBuilder.CreateBox(`lane-${i}-${x}`, { width: 0.07, height: 0.03, depth: ROAD_SEGMENT_LENGTH }, this.scene);
        lane.parent = segment;
        lane.position.set(x, -0.04, 0);
        lane.material = laneMat;
      }
      for (const x of [-6.2, 6.2]) {
        const rail = MeshBuilder.CreateBox(`rail-${i}-${x}`, { width: 0.3, height: 0.62, depth: ROAD_SEGMENT_LENGTH }, this.scene);
        rail.parent = segment;
        rail.position.set(x, 0.2, 0);
        rail.material = railMat;
        const strip = MeshBuilder.CreateBox(`rail-strip-${i}-${x}`, { width: 0.34, height: 0.06, depth: ROAD_SEGMENT_LENGTH * 0.94 }, this.scene);
        strip.parent = segment;
        strip.position.set(x, 0.47, 0);
        strip.material = laneMat;
      }
      const side = MeshBuilder.CreateBox(`sidewalk-${i}`, { width: 3.4, height: 0.16, depth: ROAD_SEGMENT_LENGTH }, this.scene);
      side.parent = segment;
      side.position.set(-8.1, -0.25, 0);
      side.material = edgeMat;
      const side2 = side.clone(`sidewalk-right-${i}`)!;
      side2.parent = segment;
      side2.position.x = 8.1;
      this.roadSegments.push(segment);
    }

    for (let i = 0; i < 22; i += 1) {
      const cluster = new TransformNode(`city-${i}`, this.scene);
      const side = i % 2 === 0 ? -1 : 1;
      cluster.position.set(side * (10 + (i % 3) * 3), 0, i * 8 - 18);
      const block = MeshBuilder.CreateBox(`building-${i}`, { width: 2.8 + (i % 3), height: 5 + ((i * 7) % 10), depth: 2.8 + ((i + 1) % 3) }, this.scene);
      block.parent = cluster;
      block.position.y = 2.1;
      block.material = this.makeMaterial(`building-mat-${i}`, i % 3 === 0 ? "#0E1B31" : "#101827", i % 4 === 0 ? "#082F48" : "#071426");
      if (i % 4 === 0) {
        const billboard = MeshBuilder.CreatePlane(`billboard-${i}`, { width: 2.2, height: 1.1 }, this.scene);
        billboard.parent = cluster;
        billboard.position.set(-side * 1.7, 3.3, 0);
        billboard.rotation.y = side * Math.PI / 2;
        billboard.material = magentaMat;
      }
      this.cityBlocks.push(cluster);
    }
  }

  private createPlayer() {
    this.player.position.set(0, 0, 0);
    const bodyMat = this.makeMaterial("player-mat", "#1B2837", "#0D3146");
    const accentMat = this.makeMaterial("player-accent", "#0E637A", "#42E8FF");
    this.playerBody = MeshBuilder.CreateCapsule("courier-body", { height: 2.4, radius: 0.43, tessellation: 12 }, this.scene);
    this.playerBody.parent = this.player;
    this.playerBody.position.y = 1.04;
    this.playerBody.material = bodyMat;
    const visor = MeshBuilder.CreateBox("courier-visor", { width: 0.6, height: 0.18, depth: 0.16 }, this.scene);
    visor.parent = this.player;
    visor.position.set(0, 1.66, -0.4);
    visor.material = accentMat;
    const trail = MeshBuilder.CreateBox("courier-trail", { width: 0.82, height: 0.03, depth: 1.35 }, this.scene);
    trail.parent = this.player;
    trail.position.set(0, 0.12, -0.86);
    trail.material = accentMat;
    this.playerTrail = trail;
    this.shieldRing = MeshBuilder.CreateTorus("shield-ring", { diameter: 1.75, thickness: 0.06, tessellation: 28 }, this.scene);
    this.shieldRing.parent = this.player;
    this.shieldRing.position.y = 1.05;
    this.shieldRing.rotation.x = Math.PI / 2;
    this.shieldRing.material = this.makeMaterial("shield-mat", "#8D6A11", "#FFC857");
    this.shieldRing.setEnabled(false);
    this.applyCharacter(this.save.snapshot.selectedCharacter);
  }

  private createPools() {
    for (let i = 0; i < 12; i += 1) this.obstacles.push(this.makeObstacle(i, i % 2 === 0 ? "barrier" : "drone"));
    for (let i = 0; i < 40; i += 1) this.coins.push(this.makeCoin(i));
    for (let i = 0; i < 4; i += 1) this.shields.push(this.makeShield(i));
  }

  private makeObstacle(index: number, kind: ObstacleKind): MovingEntity {
    const root = new TransformNode(`obstacle-root-${index}`, this.scene);
    const warning = this.makeMaterial(`warning-${index}`, "#783414", "#FF7A20");
    const danger = this.makeMaterial(`danger-${index}`, "#2E1537", "#FF4FD8");
    if (kind === "barrier") {
      const beam = MeshBuilder.CreateBox(`barrier-beam-${index}`, { width: 2.45, height: 0.64, depth: 0.32 }, this.scene);
      beam.parent = root;
      beam.position.y = 0.72;
      beam.material = warning;
      for (const x of [-1.02, 1.02]) {
        const foot = MeshBuilder.CreateBox(`barrier-foot-${index}-${x}`, { width: 0.35, height: 0.95, depth: 0.48 }, this.scene);
        foot.parent = root;
        foot.position.set(x, 0.38, 0);
        foot.material = warning;
      }
    } else {
      const core = MeshBuilder.CreateBox(`drone-core-${index}`, { width: 1.35, height: 0.35, depth: 0.9 }, this.scene);
      core.parent = root;
      core.position.y = 1.2;
      core.material = danger;
      for (const x of [-0.78, 0.78]) {
        const rotor = MeshBuilder.CreateCylinder(`drone-rotor-${index}-${x}`, { height: 0.12, diameter: 0.68, tessellation: 18 }, this.scene);
        rotor.parent = root;
        rotor.position.set(x, 1.2, 0);
        rotor.material = danger;
      }
    }
    root.setEnabled(false);
    return { root, lane: 0, active: false, kind };
  }

  private makeCoin(index: number): MovingEntity {
    const root = new TransformNode(`coin-root-${index}`, this.scene);
    const coin = MeshBuilder.CreateTorus(`coin-${index}`, { diameter: 0.62, thickness: 0.12, tessellation: 18 }, this.scene);
    coin.parent = root;
    coin.rotation.x = Math.PI / 2;
    coin.material = this.makeMaterial(`coin-mat-${index}`, "#147692", "#42E8FF");
    root.setEnabled(false);
    return { root, lane: 0, active: false, spin: Math.random() * 5 };
  }

  private makeShield(index: number): MovingEntity {
    const root = new TransformNode(`shield-root-${index}`, this.scene);
    const aura = MeshBuilder.CreateTorus(`shield-${index}`, { diameter: 1.06, thickness: 0.16, tessellation: 24 }, this.scene);
    aura.parent = root;
    aura.rotation.x = Math.PI / 2;
    aura.material = this.makeMaterial(`shield-pickup-${index}`, "#73520C", "#FFC857");
    root.setEnabled(false);
    return { root, lane: 0, active: false, spin: Math.random() * 4 };
  }

  private bindInput(canvas: HTMLCanvasElement) {
    const keydown = (event: KeyboardEvent) => {
      if (["ArrowLeft", "a", "A"].includes(event.key)) this.moveLane(-1);
      if (["ArrowRight", "d", "D"].includes(event.key)) this.moveLane(1);
      if (["ArrowUp", "w", "W", " "].includes(event.key)) {
        event.preventDefault();
        this.jump();
      }
      if (event.key === "Escape") this.phase === "run" ? this.pause() : this.phase === "paused" ? this.resume() : undefined;
    };
    const pointerDown = (event: PointerEvent) => {
      this.audio.unlock();
      this.inputStart = { x: event.clientX, y: event.clientY };
    };
    const pointerUp = (event: PointerEvent) => {
      if (!this.inputStart) return;
      const dx = event.clientX - this.inputStart.x;
      const dy = event.clientY - this.inputStart.y;
      this.inputStart = null;
      if (Math.max(Math.abs(dx), Math.abs(dy)) < 20) return;
      if (Math.abs(dx) > Math.abs(dy)) this.moveLane(dx > 0 ? 1 : -1);
      else if (dy < 0) this.jump();
    };
    window.addEventListener("keydown", keydown);
    canvas.addEventListener("pointerdown", pointerDown);
    canvas.addEventListener("pointerup", pointerUp);
    canvas.addEventListener("pointercancel", pointerUp);
    this.scene.onDisposeObservable.add(() => {
      window.removeEventListener("keydown", keydown);
      canvas.removeEventListener("pointerdown", pointerDown);
      canvas.removeEventListener("pointerup", pointerUp);
      canvas.removeEventListener("pointercancel", pointerUp);
    });
  }

  private update() {
    if (this.disposed) return;
    const dt = Math.min(this.scene.getEngine().getDeltaTime() / 1000, 0.05);
    this.updatePlayer(dt);
    if (this.phase !== "run") return;
    this.speed = Math.min(30, 15 + this.scoreDistance * 0.045);
    this.scoreDistance += this.speed * dt;
    this.spawnTimer -= dt;
    if (this.spawnTimer <= 0) {
      this.spawnWave();
      this.spawnTimer = Math.max(0.82, 1.62 - this.scoreDistance * 0.004);
    }
    this.moveWorld(dt);
    this.updateEntities(dt);
    this.shieldSeconds = Math.max(0, this.shieldSeconds - dt);
    this.shieldRing.setEnabled(this.shieldSeconds > 0);
    if (this.shieldSeconds > 0) this.shieldRing.rotation.z += dt * 3.2;
    if (this.demo) this.runDemo(dt);
    this.uiTimer -= dt;
    if (this.uiTimer <= 0) {
      this.emitState();
      this.uiTimer = 0.08;
    }
  }

  private updatePlayer(dt: number) {
    const targetX = LANES[this.laneIndex];
    this.player.position.x += (targetX - this.player.position.x) * Math.min(1, dt * 13);
    if (this.phase === "run") {
      this.verticalVelocity -= 17.5 * dt;
      this.playerY = Math.max(0, this.playerY + this.verticalVelocity * dt);
      if (this.playerY === 0 && this.verticalVelocity < 0) this.verticalVelocity = 0;
      const bob = Math.sin(this.scoreDistance * 1.1) * 0.04;
      this.player.position.y = this.playerY + bob;
      this.playerBody.rotation.z = (targetX - this.player.position.x) * -0.055;
      this.playerTrail.scaling.z = 1 + Math.min(0.45, this.speed / 70);
    } else {
      this.player.position.y *= 0.9;
    }
  }

  private moveWorld(dt: number) {
    const distance = this.speed * dt;
    for (const segment of this.roadSegments) {
      segment.position.z -= distance;
      if (segment.position.z < -30) segment.position.z += ROAD_SEGMENT_LENGTH * ROAD_SEGMENTS;
    }
    for (const city of this.cityBlocks) {
      city.position.z -= distance * 0.84;
      if (city.position.z < -36) city.position.z += 176;
    }
    this.camera.target.x += (this.player.position.x * 0.22 - this.camera.target.x) * Math.min(1, dt * 4);
    this.camera.target.y = 0.62 + this.player.position.y * 0.08;
  }

  private updateEntities(dt: number) {
    const distance = this.speed * dt;
    for (const obstacle of this.obstacles) {
      if (!obstacle.active) continue;
      obstacle.root.position.z -= distance;
      if (obstacle.kind === "drone") obstacle.root.position.y = Math.sin(this.scoreDistance * 0.9 + obstacle.root.position.z) * 0.12;
      if (obstacle.root.position.z < -8) this.hideEntity(obstacle);
      else if (Math.abs(obstacle.root.position.z) < 1.12 && Math.abs(obstacle.root.position.x - this.player.position.x) < 1.05 && this.playerY < 1.25) {
        if (this.shieldSeconds > 0) {
          this.hideEntity(obstacle);
          this.message = "SHIELD BREAK";
          this.audio.play("shield");
        } else {
          this.crash();
        }
      }
    }
    for (const coin of this.coins) {
      if (!coin.active) continue;
      coin.root.position.z -= distance;
      coin.root.rotation.y += dt * 6;
      coin.root.position.y = 1.05 + Math.sin(this.scoreDistance * 2 + (coin.spin ?? 0)) * 0.12;
      if (coin.root.position.z < -8) this.hideEntity(coin);
      else if (Math.abs(coin.root.position.z) < 1.2 && Math.abs(coin.root.position.x - this.player.position.x) < 0.82 && Math.abs(this.playerY - 0.1) < 1.7) {
        this.runCoins += 1;
        this.hideEntity(coin);
        this.audio.play("coin");
      }
    }
    for (const shield of this.shields) {
      if (!shield.active) continue;
      shield.root.position.z -= distance;
      shield.root.rotation.y += dt * 2.4;
      if (shield.root.position.z < -8) this.hideEntity(shield);
      else if (Math.abs(shield.root.position.z) < 1.3 && Math.abs(shield.root.position.x - this.player.position.x) < 0.95) {
        this.shieldSeconds = 6.5;
        this.hideEntity(shield);
        this.message = "SHIELD ONLINE";
        this.audio.play("shield");
      }
    }
  }

  private spawnWave() {
    const seed = Math.floor(this.scoreDistance * 10 + this.runCoins * 7) % 5;
    const lanes = [0, 1, 2];
    const safeLane = (Math.floor(this.scoreDistance / 8) + seed) % 3;
    const startZ = 52;
    if (seed === 0) {
      this.activateObstacle(lanes.filter((lane) => lane !== safeLane)[0], startZ, "barrier");
      this.spawnCoins(safeLane, startZ + 1.8, 4);
    } else if (seed === 1) {
      this.activateObstacle(safeLane, startZ, "drone");
      this.spawnCoins((safeLane + 1) % 3, startZ - 1, 5);
    } else if (seed === 2) {
      this.activateObstacle((safeLane + 1) % 3, startZ, "barrier");
      this.activateObstacle((safeLane + 2) % 3, startZ + 9, "drone");
      this.spawnCoins(safeLane, startZ + 2, 5);
    } else if (seed === 3) {
      this.spawnCoins(safeLane, startZ, 6);
      if (this.scoreDistance > 38) this.activateShield((safeLane + 1) % 3, startZ + 7);
    } else {
      this.activateObstacle((safeLane + 1) % 3, startZ, "drone");
      this.activateShield(safeLane, startZ + 3);
      this.spawnCoins((safeLane + 2) % 3, startZ + 5, 4);
    }
  }

  private getInactive(pool: MovingEntity[]) {
    return pool.find((item) => !item.active) ?? pool[0];
  }

  private activateObstacle(lane: number, z: number, kind: ObstacleKind) {
    const entity = this.obstacles.find((item) => !item.active && item.kind === kind) ?? this.getInactive(this.obstacles);
    entity.lane = lane;
    entity.active = true;
    entity.root.position.set(LANES[lane], 0, z);
    entity.root.setEnabled(true);
  }

  private spawnCoins(lane: number, z: number, count: number) {
    for (let i = 0; i < count; i += 1) {
      const coin = this.getInactive(this.coins);
      coin.lane = lane;
      coin.active = true;
      coin.root.position.set(LANES[lane], 1.05, z + i * 2.15);
      coin.root.setEnabled(true);
    }
  }

  private activateShield(lane: number, z: number) {
    const entity = this.getInactive(this.shields);
    entity.lane = lane;
    entity.active = true;
    entity.root.position.set(LANES[lane], 1.15, z);
    entity.root.setEnabled(true);
  }

  private hideEntity(entity: MovingEntity) {
    entity.active = false;
    entity.root.setEnabled(false);
  }

  private runDemo(dt: number) {
    this.demoTimer -= dt;
    if (this.demoTimer > 0) return;
    const threat = this.obstacles.find((item) => item.active && item.root.position.z > 0 && item.root.position.z < 18 && item.lane === this.laneIndex);
    if (threat) {
      const open = [0, 1, 2].find((lane) => lane !== threat.lane && !this.obstacles.some((item) => item.active && item.root.position.z > 0 && item.root.position.z < 18 && item.lane === lane));
      if (open !== undefined) this.laneIndex = open;
      else this.jump();
    } else {
      const nearbyCoin = this.coins.find((item) => item.active && item.root.position.z > 4 && item.root.position.z < 19);
      if (nearbyCoin) this.laneIndex = nearbyCoin.lane;
    }
    this.demoTimer = 0.28;
  }

  start = () => {
    this.audio.unlock();
    this.audio.play("ui");
    this.phase = "run";
    this.resetRun();
    this.emitState();
  };

  pause = () => {
    if (this.phase !== "run") return;
    this.phase = "paused";
    this.audio.play("ui");
    this.emitState();
  };

  resume = () => {
    if (this.phase !== "paused") return;
    this.phase = "run";
    this.audio.play("ui");
    this.emitState();
  };

  retry = () => {
    this.phase = "run";
    this.resetRun();
    this.audio.play("ui");
    this.emitState();
  };

  home = () => {
    this.phase = "menu";
    this.deactivateAll();
    this.audio.play("ui");
    this.emitState();
  };

  openShop = () => {
    this.phase = "shop";
    this.audio.play("ui");
    this.emitState();
  };

  closeShop = () => {
    this.phase = "menu";
    this.audio.play("ui");
    this.emitState();
  };

  selectCharacter = (id: number) => {
    const character = CHARACTERS.find((item) => item.id === id);
    const save = this.save.snapshot;
    if (!character) return;
    if (save.unlockedCharacters.includes(id)) {
      this.save.update({ selectedCharacter: id });
      this.applyCharacter(id);
      this.message = `${character.name} SELECTED`;
      this.audio.play("ui");
    } else if (save.coins >= character.price) {
      this.save.update({ coins: save.coins - character.price, unlockedCharacters: [...save.unlockedCharacters, id], selectedCharacter: id });
      this.applyCharacter(id);
      this.message = `${character.name} UNLOCKED`;
      this.audio.play("shield");
    } else {
      this.message = "INSUFFICIENT CREDITS";
      this.audio.play("hit");
    }
    this.emitState();
  };

  revive = () => {
    if (this.phase !== "gameOver" || !this.canRevive()) return;
    this.revived = true;
    this.phase = "run";
    this.shieldSeconds = 5;
    this.scoreDistance = Math.max(0, this.scoreDistance - 8);
    this.message = "RELAY REVIVE // DEMO";
    this.audio.play("shield");
    this.emitState();
  };

  doubleReward = () => {
    if (this.phase !== "gameOver" || this.doubleClaimed) return;
    const save = this.save.snapshot;
    this.save.update({ coins: save.coins + this.runCoins });
    this.doubleClaimed = true;
    this.message = "REWARD DOUBLED // DEMO";
    this.audio.play("coin");
    this.emitState();
  };

  toggleMusic = () => {
    const enabled = !this.save.snapshot.musicEnabled;
    this.save.update({ musicEnabled: enabled });
    this.audio.setEnabled(enabled);
    if (enabled) this.audio.play("ui");
    this.emitState();
  };

  private moveLane(direction: number) {
    if (this.phase !== "run") return;
    const next = Math.max(0, Math.min(2, this.laneIndex + direction));
    if (next === this.laneIndex) return;
    this.laneIndex = next;
    this.audio.play("move");
  }

  private jump() {
    if (this.phase !== "run" || this.playerY > 0.05) return;
    this.verticalVelocity = 7.8;
    this.audio.play("jump");
  }

  private crash() {
    if (this.phase !== "run") return;
    this.phase = "gameOver";
    const score = Math.floor(this.scoreDistance * 10);
    const save = this.save.snapshot;
    this.interstitialReady = save.runsSinceInterstitial + 1 >= 3;
    this.save.update({
      coins: save.coins + this.runCoins,
      bestScore: Math.max(save.bestScore, score),
      runsSinceInterstitial: this.interstitialReady ? 0 : save.runsSinceInterstitial + 1,
    });
    this.message = this.interstitialReady ? "TRANSIT BREAK // OPTIONAL DEMO" : "RUN TERMINATED";
    this.audio.play("hit");
    this.emitState();
  }

  private resetRun() {
    this.deactivateAll();
    this.laneIndex = 1;
    this.player.position.set(0, 0, 0);
    this.playerY = 0;
    this.verticalVelocity = 0;
    this.shieldSeconds = 0;
    this.scoreDistance = 0;
    this.runCoins = 0;
    this.speed = 15;
    this.spawnTimer = 0.65;
    this.revived = false;
    this.doubleClaimed = false;
    this.message = null;
  }

  private deactivateAll() {
    [...this.obstacles, ...this.coins, ...this.shields].forEach((entity) => this.hideEntity(entity));
  }

  private canRevive() {
    return !this.revived && this.scoreDistance > 16;
  }

  private applyCharacter(id: number) {
    const character = CHARACTERS.find((item) => item.id === id) ?? CHARACTERS[0];
    const material = this.playerBody.material as StandardMaterial;
    material.emissiveColor = Color3.FromHexString(character.accent).scale(0.38);
    const trailMaterial = this.playerTrail.material as StandardMaterial;
    trailMaterial.emissiveColor = Color3.FromHexString(character.accent);
  }

  private emitState() {
    const save = this.save.snapshot;
    const state: UIState = {
      phase: this.phase,
      score: Math.floor(this.scoreDistance * 10),
      bestScore: save.bestScore,
      coins: save.coins,
      runCoins: this.runCoins,
      selectedCharacter: save.selectedCharacter,
      unlockedCharacters: save.unlockedCharacters,
      musicEnabled: save.musicEnabled,
      shieldSeconds: this.shieldSeconds,
      canRevive: this.canRevive(),
      interstitialReady: this.interstitialReady,
      message: this.message,
    };
    this.bridge.onState(state);
  }

  dispose = () => {
    this.disposed = true;
    this.scene.dispose();
  };
}
