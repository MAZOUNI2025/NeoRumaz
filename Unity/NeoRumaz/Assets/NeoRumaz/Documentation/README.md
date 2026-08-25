# NeoRumaz Unity 3D

This directory is the production Unity project. It was initialized from the complete LetsRun Unity 2020.3.26f1 source as a preserved technical baseline, then given an independent NeoRumaz runtime and scene under `Assets/NeoRumaz/`.

Open `Unity/NeoRumaz/` in **Unity 2020.3.26f1** or a compatible Unity LTS editor, allow package resolution, and open `Assets/NeoRumaz/Scenes/NeoRumazGame.unity`. The build settings make this the first scene.

The runtime generates a three-lane third-person runner, city highway, courier, city blocks, barriers, drones, credits, `Scarab Shield`, `Nile Rush`, `Cairo Contract`, pooling, touch/keyboard input, and English-first screens for the main menu, runner garage, active run, and results. Local profile source covers credits, high score, total runs, unlock state, selected runner, and audio preference. The runtime does not rely on the Web prototype.

`ProfileProgression.cs`, `GameplayConfiguration.cs`, and `MonetizationArchitecture.cs` isolate long-term progression, tunable balance, and provider-neutral advertising from the gameplay loop. The current monetization provider is intentionally a no-op implementation: no SDK, app ID, key, secret, or real placement is stored in source.

## Android status

Android settings are prepared as project configuration only. This workspace has no Unity Editor, Android Build Support, Android SDK/NDK, or device bridge, so no APK/AAB has been produced or verified. The legacy LetsRun advertising package was removed from the production manifest; no real advertising SDK is integrated.
