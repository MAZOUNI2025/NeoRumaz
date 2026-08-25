// Neon Courier Transit reminder: React frames the game; the center remains clear for fair three-lane decisions.

import { useEffect, useRef, useState } from "react";
import { Engine } from "@babylonjs/core/Engines/engine";
import { createGameScene, type GameHandle } from "@/game/scene";
import { CHARACTERS, type UIState } from "@/game/types";
import { Home, LockKeyhole, Pause, Play, RotateCcw, Shield, Volume2, VolumeX, X, Zap } from "lucide-react";

const MENU_IMAGE = "/manus-storage/neorumaz-menu-city_9f20a22d.jpg";
const SYMBOL = "/manus-storage/neorumaz-symbol_49b3ba69.png";

const initialState: UIState = {
  phase: "menu",
  score: 0,
  bestScore: 0,
  coins: 450,
  runCoins: 0,
  selectedCharacter: 0,
  unlockedCharacters: [0],
  musicEnabled: true,
  shieldSeconds: 0,
  canRevive: false,
  interstitialReady: false,
  message: null,
};

function Panel({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <div className={`nr-panel ${className}`}>{children}</div>;
}

export default function GameCanvas() {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const startedRef = useRef(false);
  const handleRef = useRef<GameHandle | null>(null);
  const [state, setState] = useState<UIState>(initialState);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas || startedRef.current) return;
    startedRef.current = true;
    const engine = new Engine(canvas, true, { preserveDrawingBuffer: true, stencil: true, adaptToDeviceRatio: true });
    let destroyed = false;
    createGameScene(engine, canvas, {
      onState: (next) => {
        if (!destroyed) setState(next);
      },
    }).then((handle) => {
      if (destroyed) {
        handle.dispose();
        return;
      }
      handleRef.current = handle;
      engine.runRenderLoop(() => handle.scene.render());
    });
    const resize = () => engine.resize();
    window.addEventListener("resize", resize);
    return () => {
      destroyed = true;
      window.removeEventListener("resize", resize);
      handleRef.current?.dispose();
      handleRef.current = null;
      engine.dispose();
      startedRef.current = false;
    };
  }, []);

  const game = handleRef.current;
  const selected = CHARACTERS[state.selectedCharacter];
  const isMenu = state.phase === "menu" || state.phase === "shop";

  return (
    <div className="nr-game-shell">
      <canvas ref={canvasRef} className="nr-canvas" style={{ touchAction: "none" }} />
      {isMenu && <div className="nr-menu-backdrop" style={{ backgroundImage: `linear-gradient(90deg, rgba(4, 8, 18, .93) 0%, rgba(4, 8, 18, .54) 46%, rgba(4, 8, 18, .18) 100%), url(${MENU_IMAGE})` }} />}
      {isMenu && <div className="nr-route-field" aria-hidden="true"><i /><i /><i /><i /></div>}

      <div className="nr-ui" aria-live="polite">
        {state.phase === "menu" && (
          <section className="nr-menu">
            <div className="nr-brand-lockup">
              <img src={SYMBOL} alt="NeoRumaz route symbol" className="nr-symbol" />
              <div>
                <p className="nr-eyebrow">NIGHT TRANSIT // 01</p>
                <h1>NEO<span>RUMAZ</span></h1>
              </div>
            </div>
            <div className="nr-menu-copy">
              <p className="nr-kicker">PREMIUM ENDLESS RUNNER</p>
              <h2>THE CITY<br />DOESN&apos;T WAIT.</h2>
              <p dir="rtl">تجنب المسارات المحظورة، اجمع شحنات العبور، وارفع سرعتك قبل أن يغلق عليك الطريق.</p>
            </div>
            <div className="nr-menu-actions">
              <button className="nr-primary" onClick={() => game?.start()}><Play size={18} fill="currentColor" /> START RUN</button>
              <button className="nr-secondary" onClick={() => game?.openShop()}><Zap size={17} /> RUNNER GARAGE</button>
              <button className="nr-audio-button" onClick={() => game?.toggleMusic()} aria-label="Toggle sound">
                {state.musicEnabled ? <Volume2 size={18} /> : <VolumeX size={18} />}
                SOUND {state.musicEnabled ? "ON" : "OFF"}
              </button>
            </div>
            <div className="nr-menu-status">
              <div><small>BEST ROUTE</small><strong>{state.bestScore.toLocaleString()}</strong></div>
              <div><small>TRANSIT CREDITS</small><strong className="nr-cyan">◈ {state.coins.toLocaleString()}</strong></div>
              <div><small>ACTIVE RUNNER</small><strong style={{ color: selected.accent }}>{selected.name}</strong></div>
            </div>
            <p className="nr-control-note">SWIPE ← → TO CHANGE LANE &nbsp; · &nbsp; SWIPE ↑ TO JUMP</p>
          </section>
        )}

        {state.phase === "shop" && (
          <section className="nr-garage" aria-label="Runner garage">
            <header className="nr-garage-head">
              <button className="nr-icon-button" onClick={() => game?.closeShop()} aria-label="Close garage"><X size={19} /></button>
              <div>
                <p className="nr-eyebrow">RUNNER GARAGE</p>
                <h2>CHOOSE YOUR ROUTE</h2>
              </div>
              <Panel className="nr-credit-panel"><span>◈</span><strong>{state.coins.toLocaleString()}</strong></Panel>
            </header>
            <div className="nr-character-grid">
              {CHARACTERS.map((character) => {
                const unlocked = state.unlockedCharacters.includes(character.id);
                const selectedCharacter = state.selectedCharacter === character.id;
                return (
                  <article className={`nr-character-card ${selectedCharacter ? "is-selected" : ""}`} key={character.id} style={{ "--character-accent": character.accent } as React.CSSProperties}>
                    <div className="nr-card-topline"><span>{character.role}</span>{selectedCharacter && <b>ACTIVE</b>}</div>
                    <img src={character.portrait} alt={`${character.name} courier`} />
                    <div className="nr-character-info"><h3>{character.name}</h3><p>{unlocked ? "COSMETIC RUNNER" : "LOCKED ROUTE"}</p></div>
                    <button onClick={() => game?.selectCharacter(character.id)} className={selectedCharacter ? "nr-selected-button" : "nr-card-button"}>
                      {selectedCharacter ? "SELECTED" : unlocked ? "SELECT" : <><LockKeyhole size={14} /> {character.price.toLocaleString()} ◈</>}
                    </button>
                  </article>
                );
              })}
            </div>
            <p className="nr-garage-foot">كل Runner يغيّر المظهر فقط. الأداء العادل ثابت في كل المسارات.</p>
          </section>
        )}

        {state.phase === "run" && (
          <>
            <div className="nr-hud nr-hud-left"><Panel><span className="nr-hud-label">SCORE</span><strong>{state.score.toLocaleString()}</strong><div className="nr-progress"><i style={{ width: `${Math.min(100, (state.score % 500) / 5)}%` }} /></div></Panel></div>
            <div className="nr-hud nr-hud-right"><Panel className="nr-coin-hud"><span>◈</span><strong>{state.coins.toLocaleString()}</strong>{state.shieldSeconds > 0 && <em><Shield size={14} /> {state.shieldSeconds.toFixed(1)}s</em>}</Panel></div>
            <button className="nr-pause-button" onClick={() => game?.pause()} aria-label="Pause run"><Pause size={22} fill="currentColor" /></button>
            <div className="nr-jump-hint"><span>↑</span><small>JUMP</small></div>
          </>
        )}

        {state.phase === "paused" && (
          <div className="nr-modal-backdrop"><section className="nr-modal"><p className="nr-eyebrow">TRANSIT PAUSED</p><h2>HOLD YOUR LINE.</h2><p>المسار ينتظر قرارك. استأنف عندما تكون مستعدًا.</p><button className="nr-primary" onClick={() => game?.resume()}><Play size={17} fill="currentColor" /> RESUME RUN</button><button className="nr-text-button" onClick={() => game?.home()}><Home size={16} /> RETURN TO HUB</button></section></div>
        )}

        {state.phase === "gameOver" && (
          <div className="nr-modal-backdrop"><section className="nr-modal nr-gameover"><p className="nr-eyebrow">ROUTE INTERRUPTED</p><h2>RUN COMPLETE</h2><div className="nr-result-grid"><div><small>DISTANCE SCORE</small><strong>{state.score.toLocaleString()}</strong></div><div><small>RUN CREDITS</small><strong className="nr-cyan">+{state.runCoins}</strong></div></div>{state.interstitialReady && <p className="nr-ad-note">TRANSIT BREAK AVAILABLE — محاكاة إعلان انتقالية اختيارية، ولا تؤثر على إعادة اللعب.</p>}{state.canRevive && <button className="nr-revive" onClick={() => game?.revive()}><Shield size={18} /> REVIVE RELAY <small>DEMO</small></button>}<button className="nr-primary" onClick={() => game?.retry()}><RotateCcw size={17} /> RUN AGAIN</button>{state.runCoins > 0 && <button className="nr-secondary nr-double" onClick={() => game?.doubleReward()}><Zap size={16} /> DOUBLE CREDITS <small>DEMO</small></button>}<button className="nr-text-button" onClick={() => game?.home()}><Home size={16} /> RETURN TO HUB</button></section></div>
        )}

        {state.message && <div className="nr-toast">{state.message}</div>}
      </div>
    </div>
  );
}
