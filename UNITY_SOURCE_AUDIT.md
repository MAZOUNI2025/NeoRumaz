# NeoRumaz Unity Build-Readiness Audit

**Audit date:** 25 August 2026  
**Scope:** Source-level audit of `Unity/NeoRumaz/`. Unity Editor, Android Build Support, Android SDK/NDK/OpenJDK, emulator, and Android device tooling are not available in this workspace.

## Status Gate

| State | Status | Meaning |
| --- | --- | --- |
| **SOURCE READY** | **Yes, statically audited** | Repository structure, source metadata, package JSON, scene path, Android source settings, and C# syntax have been checked. |
| **BUILD READY** | **Pending Unity Build Automation import** | The source is configured for import, but only Unity Editor/Build Automation can confirm package resolution and compilation. |
| **APK BUILT** | **No** | No APK or AAB has been generated or inspected. |
| **ANDROID RUNTIME VERIFIED** | **No** | No device or emulator has run the game. |

## Required Build Automation Root

> **Unity Build Automation Project Subdirectory must be `Unity/NeoRumaz`.**

That folder directly contains `Assets/`, `Packages/`, and `ProjectSettings/`. The earlier `unrecognized project` failure is consistent with the dashboard pointing at the repository root or another incorrect subdirectory. This dashboard selection cannot be changed from Git; see [`UNITY_BUILD_CONFIGURATION.md`](UNITY_BUILD_CONFIGURATION.md).

## Final Source Checklist

| Check | Status | Static evidence |
| --- | --- | --- |
| Correct Unity project root | [x] | `Unity/NeoRumaz/` directly contains the three Unity project directories. |
| Assets directory | [x] | `Unity/NeoRumaz/Assets/` exists. |
| Packages directory | [x] | `Unity/NeoRumaz/Packages/` exists. |
| ProjectSettings directory | [x] | `Unity/NeoRumaz/ProjectSettings/` exists. |
| ProjectVersion.txt | [x] | Declares Unity `2020.3.26f1 (7298b473bc1a)`. |
| Valid package JSON | [x] | `manifest.json` parses as JSON. |
| Valid packages lock | [x] | `packages-lock.json` parses as JSON. |
| Build scene exists | [x] | `Assets/NeoRumaz/Scenes/NeoRumazGame.unity` exists. |
| Build scene configured | [x] | `NeoRumazGame` is the first enabled build scene. |
| C# static validation | [x] | Tree-sitter parsed 21 C# files without syntax errors. |
| Meta/GUID validation | [x] | Required C#/scene/prefab/asset meta files exist; no duplicate NeoRumaz GUIDs found. |
| Prefab validation | [x] | Runtime prefab, gameplay prefabs, catalog references, and script GUID links resolve statically. |
| Android configuration | [x] | Landscape/full screen, API 24 minimum, auto target SDK, IL2CPP, ARMv7 + ARM64, no forced Internet permission. |
| Bundle ID | [x] | `com.neorumaz.runner`. |
| No obsolete Ads/Purchasing references | [x] | Neither package nor live C# provider call remains. |
| No generated Unity folders committed | [x] | `Library`, `Temp`, `Obj`, `Logs`, and `UserSettings` are absent and ignored. |
| Build Automation configuration documented | [x] | `UNITY_BUILD_CONFIGURATION.md` records the required dashboard values. |

## Source-Side Findings and Fixes

| Finding | Source-side action |
| --- | --- |
| Unity Build Automation failed to identify the project. | Verified the Unity root and documented the exact dashboard Project Subdirectory: `Unity/NeoRumaz`. |
| Android target architectures were ARMv7-only. | Set `AndroidTargetArchitectures: 3` for ARMv7 + ARM64. |
| Android scripting backend was unspecified/default. | Set the Android backend to IL2CPP, which is appropriate for the configured ARM64 target. |
| A non-empty legacy PS4 passcode existed in `ProjectSettings.asset`. | Cleared the value so no secret-like legacy value remains committed. |
| Legacy Unity Ads/Purchasing entries previously existed. | Kept both absent from manifest and lock; the provider-neutral monetization architecture remains disabled by default. |

## What Was Actually Verified

The audit inspected the physical project tree, Unity version declaration, build-scene path/order, package JSON syntax and direct-package uniqueness, C# syntax, required asset presence, meta completeness, GUID uniqueness, catalog GUID resolution, generated-directory absence, advertisement/purchasing references, Android serialized fields, and sensitive-text checks. No Unity compilation was performed.

## What Still Requires Unity Editor

Unity Editor must import `Unity/NeoRumaz/`, resolve packages, compile every assembly against Unity `2020.3.26f1`, deserialize the scene and prefabs, run `NeoRumaz > Validate Project Source`, and execute an Android build. Build Automation must also be configured with the exact Project Subdirectory above.

## What Still Requires a Real Android Device

A real device test must verify installation, launch, touch and swipe input, lane movement, jump timing, rendering, frame pacing, audio, safe-area behavior, persistence, crash handling, and final APK signing/compatibility. Monetization remains intentionally unverified because no Android-compatible provider SDK or device test has been integrated.
