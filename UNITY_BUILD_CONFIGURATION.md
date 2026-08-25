# NeoRumaz Unity Build Automation Configuration

This file documents the **dashboard configuration** required for Unity Build Automation. The previous `unrecognized project` error is a project-subdirectory detection problem: Build Automation was not pointed at the folder that directly contains Unity's `Assets`, `Packages`, and `ProjectSettings` directories. A Git commit cannot change this dashboard setting.

## Required Dashboard Values

| Setting | Required value |
| --- | --- |
| Repository | `MAZOUNI2025/NeoRumaz` |
| Branch | `main` |
| **Project Subdirectory** | **`Unity/NeoRumaz`** |
| Unity version | `2020.3.26f1` (`7298b473bc1a`) |
| Builder OS | Windows 11 24H2, or the closest available Windows image that supports Unity `2020.3.26f1` |
| Build target | Android |
| Android SDK/NDK/OpenJDK | The Android Build Support toolchain compatible with Unity `2020.3.26f1`; do not select an arbitrary newer SDK independently of the selected Unity image. |
| Product name | `NeoRumaz` |
| Bundle ID | `com.neorumaz.runner` |
| Initial device-test artifact | APK |
| Build App Bundle (AAB) | Off |
| Auto Build | Off |
| Development Build | Off, unless a debugging session explicitly requires it |

## Source Configuration Present in the Repository

The Unity project root is `Unity/NeoRumaz/`. It directly contains `Assets/`, `Packages/`, `ProjectSettings/`, and `ProjectSettings/ProjectVersion.txt`. The first enabled build scene is `Assets/NeoRumaz/Scenes/NeoRumazGame.unity`.

Android source settings are currently configured for landscape gameplay, full-screen launch, package identifier `com.neorumaz.runner`, Android minimum API level 24, automatic target SDK selection for the installed Unity toolchain, ARMv7 + ARM64 architectures, and IL2CPP. Internet permission is not forced by the source project because no live network or advertising SDK is included.

## Mandatory First Build Procedure

1. Set **Project Subdirectory** to exactly `Unity/NeoRumaz` in Build Automation before starting a build.
2. Use the exact Unity editor version above, with Android Build Support available on the selected builder image.
3. Build an unsigned or securely signed test APK as appropriate for the pipeline; do not commit keystores, aliases, passwords, or provider credentials.
4. If Unity imports the project, run `NeoRumaz > Validate Project Source` locally or in a pre-build-capable workflow, then review the generated Editor/build logs.
5. Treat any successful source detection as only the start of validation. It does not prove runtime input, rendering, performance, monetization, or device compatibility.
