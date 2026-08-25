# NeoRumaz — Unity 3D Android Product Specification

## Product Direction

NeoRumaz is a **Unity 3D Android endless runner**, not a React or Babylon.js product. The current web implementation, its artwork, gameplay notes, and verification records remain reference material only. The production implementation lives in `Unity/NeoRumaz/`.

## Reference Audit

The verified LetsRun source is a complete Unity 2020.3.26f1 project with `MainMenu` and `Gameplay` scenes, 14 C# scripts, player and obstacle prefabs, UI and audio resources, Android-oriented project settings, and legacy Unity Ads/Purchasing package declarations. Its two-lane 2D-style presentation and simple prefab visuals are not a production visual baseline for NeoRumaz.

## Visual Quality Bar

The target look is the supplied premium reference: a third-person runner with a visible human courier, an elevated wet-finish city highway, dense night skyline, neon blue lane infrastructure, amber industrial barriers, magenta drone hazards, glowing credits, and crisp HUD panels. This is a **quality target**, not an instruction to copy a source image or use its branding.

## Gameplay Contract

| System | Unity implementation requirement |
| --- | --- |
| Movement | Three explicit lanes with tap/swipe and keyboard fallback; lane switches are smooth and bounded. |
| Jump | Grounded jump with a short forgiving input buffer; obstacles use collider-based resolution. |
| World | Recycled road segments and scenery; pooled obstacle, pickup, and VFX instances. |
| Rewards | Credits, `Scarab Shield`, `Nile Rush`, and `Cairo Contract` are real game-state systems. |
| Interface | English is the shipped command language; concise, readable, mobile-safe HUD. |
| Save | Local progress, high score, unlock state, and settings must survive restart. |

## Non-Claims

No Android APK/AAB exists yet because this environment does not have Unity Editor, Unity Android Build Support, Android SDK/NDK, or an Android device bridge. No real advertising SDK is integrated, configured, or tested. Neither claim may appear in a release checklist until a Unity Android build is produced and verified.

## Repository Structure

| Path | Role |
| --- | --- |
| `Unity/NeoRumaz/` | Production Unity 3D project. |
| `client/`, `package.json`, and related root web files | Historical Web/Babylon reference prototype; not the Android product. |
| Existing audit/design documentation | Preserved reference for game rules, quality decisions, and source attribution. |
