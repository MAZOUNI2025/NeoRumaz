# NeoRumaz Unity Source Audit

**Audit date:** 25 August 2026  
**Scope:** `Unity/NeoRumaz/` source project only. This is not runtime, device, APK, or AAB verification.

## Static Validation Result

| Area | Result | Evidence |
| --- | --- | --- |
| C# syntax | Pass | Tree-sitter parsed all 21 C# files in `Assets/` with no syntax errors. |
| Unity baseline | Pass | `ProjectVersion.txt` remains on Unity `2020.3.26f1`, matching LetsRun's audited source baseline. |
| Scene ordering | Pass | `Assets/NeoRumaz/Scenes/NeoRumazGame.unity` is the first build scene, ahead of preserved LetsRun scenes. |
| Android source settings | Pass | Product `NeoRumaz`, package ID `com.neorumaz.runner`, Android minimum API level 24. |
| Required assets | Pass | Production scene, runtime prefab, gameplay configuration, prefab catalog, monetization configuration, and Editor validator all exist. |
| Unity metadata | Pass | All NeoRumaz C#, prefab, scene, and ScriptableObject assets have `.meta` files; no duplicate GUIDs were found. |
| Prefab catalog links | Pass | Each catalog GUID resolves to a NeoRumaz source asset. |
| Generated files | Pass | `Library`, `Temp`, `Obj`, `Logs`, and `UserSettings` are absent and ignored. |
| Legacy live commerce packages | Pass | Unity Ads and Unity Purchasing are absent from `manifest.json` and `packages-lock.json`. |
| Live provider calls | Pass | No C# references to `UnityEngine.Advertisements`, `Advertisement.Initialize`, `Advertisement.Show`, or `UnityPurchasing` were found. |

## What This Audit Does Not Prove

The workspace does not have Unity Editor, Android Build Support, Android SDK/NDK/OpenJDK, an Android emulator, or a device bridge. Therefore this audit **does not prove** that Unity imports the project without error, that generated scene/prefab YAML renders as intended, that game controls work on a device, that the project meets performance requirements, or that an APK/AAB can be built.

The project includes `NeoRumaz > Validate Project Source`, an Editor pre-build validator that repeats the critical scene, asset, economy, and monetization policy checks once Unity is available.

## Required Local Follow-up

Open `Unity/NeoRumaz/` in Unity 2020.3.26f1 with Android Build Support. Let Unity resolve packages and regenerate local artifacts, run **NeoRumaz > Validate Project Source**, open `NeoRumazGame`, then play-test, profile, and build a signed Android artifact. Do not enable either monetization switch until a real provider adapter has been integrated and tested on devices.
