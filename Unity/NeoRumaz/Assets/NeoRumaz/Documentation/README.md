# NeoRumaz Unity 3D

This directory is the production Unity project. It was initialized from the complete LetsRun Unity 2020.3.26f1 source as a preserved technical baseline, then given an independent NeoRumaz runtime and scene under `Assets/NeoRumaz/`.

Open `Unity/NeoRumaz/` in **Unity 2020.3.26f1** or a compatible Unity LTS editor, allow package resolution, and open `Assets/NeoRumaz/Scenes/NeoRumazGame.unity`. The build settings make this the first scene.

The runtime generates a three-lane third-person runner, city highway, courier, city blocks, barriers, drones, credits, `Scarab Shield`, `Nile Rush`, `Cairo Contract`, pooling, touch/keyboard input, and an English HUD without relying on the Web prototype.

## Android status

Android settings are prepared as project configuration only. This workspace has no Unity Editor, Android Build Support, Android SDK/NDK, or device bridge, so no APK/AAB has been produced or verified. The legacy LetsRun advertising package was removed from the production manifest; no real advertising SDK is integrated.
