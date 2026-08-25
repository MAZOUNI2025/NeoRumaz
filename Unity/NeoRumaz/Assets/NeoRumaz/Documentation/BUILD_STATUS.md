# Unity and Android Build Status

| Check | Status | Evidence |
| --- | --- | --- |
| NeoRumaz Unity project structure | Present | `Unity/NeoRumaz/` is a full Unity project copied from the verified LetsRun baseline. |
| Production scene | Present | `Assets/NeoRumaz/Scenes/NeoRumazGame.unity` is first in Build Settings. |
| Runtime gameplay | Implemented in source | `NeoRumazBootstrap.cs` creates the 3D road, city, runner, pooled game items, touch input, and gameplay state at runtime. |
| English command HUD | Implemented in source | `NeoRumazHud.cs` creates score, credits, contract, boost, lane, jump, and retry UI. |
| Android product settings | Configured | Package ID `com.neorumaz.runner`, product name `NeoRumaz`, Android minimum API level 24. |
| Unity compile | Not performed | Unity Editor is not installed in this workspace. |
| Android APK/AAB | Not produced | Unity Android Build Support, Android SDK/NDK, and signing configuration are not installed. |
| Android device validation | Not performed | No Android device bridge or emulator is available. |
| Real advertising | Not integrated | Legacy Unity Ads dependency was removed; no replacement SDK exists. |

The project includes `NeoRumaz > Validate Project Source` under the Unity Editor menu. It also runs as a pre-build check after the local Unity project is opened. This validator has been added to source but cannot be executed in the current environment because Unity Editor is unavailable.

## Required verification sequence

1. Install Unity 2020.3.26f1 with **Android Build Support**, Android SDK/NDK Tools, and OpenJDK, or upgrade the copied project through Unity Hub and resolve packages.
2. Open `Unity/NeoRumaz/`, allow Unity to generate `Library/`, and open `Assets/NeoRumaz/Scenes/NeoRumazGame.unity`.
3. Run the scene in the editor and fix any Unity compiler or rendering warnings before setting the build target to Android.
4. Configure a real Android keystore, set an Android API level supported by the target store, and build an APK for device testing or an AAB for store submission.
5. Test swipe controls, safe areas, resume behavior, performance, crashes, and reward handling on physical Android devices before adding an ad provider.

> This document deliberately does not treat source code or Android Player Settings as evidence of a successful Android build.
