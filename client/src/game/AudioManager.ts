// Neon Courier Transit reminder: feedback is brief, restrained, and unlocked only after a user gesture.

export class AudioManager {
  private context: AudioContext | null = null;
  private enabled = true;

  constructor(enabled: boolean) {
    this.enabled = enabled;
  }

  setEnabled(enabled: boolean) {
    this.enabled = enabled;
  }

  unlock() {
    if (!this.context) {
      const AudioContextCtor = window.AudioContext;
      if (AudioContextCtor) this.context = new AudioContextCtor();
    }
    if (this.context?.state === "suspended") void this.context.resume();
  }

  play(kind: "move" | "jump" | "coin" | "hit" | "shield" | "ui") {
    if (!this.enabled) return;
    this.unlock();
    const context = this.context;
    if (!context) return;
    const tone: Record<typeof kind, [number, number, number]> = {
      move: [380, 0.045, 0.025],
      jump: [520, 0.12, 0.04],
      coin: [860, 0.06, 0.035],
      hit: [120, 0.22, 0.075],
      shield: [690, 0.18, 0.04],
      ui: [460, 0.045, 0.022],
    };
    const [frequency, duration, gain] = tone[kind];
    const oscillator = context.createOscillator();
    const amp = context.createGain();
    oscillator.type = kind === "hit" ? "sawtooth" : "sine";
    oscillator.frequency.setValueAtTime(frequency, context.currentTime);
    if (kind === "coin" || kind === "jump") oscillator.frequency.exponentialRampToValueAtTime(frequency * 1.38, context.currentTime + duration);
    amp.gain.setValueAtTime(0.0001, context.currentTime);
    amp.gain.exponentialRampToValueAtTime(gain, context.currentTime + 0.008);
    amp.gain.exponentialRampToValueAtTime(0.0001, context.currentTime + duration);
    oscillator.connect(amp).connect(context.destination);
    oscillator.start();
    oscillator.stop(context.currentTime + duration + 0.01);
  }
}
