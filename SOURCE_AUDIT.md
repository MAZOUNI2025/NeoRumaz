# NeoRumaz source findings

## Repository verification

The source repository is `https://github.com/tejasbadone/letsrun`, with 7 commits on `main`, a Unity project at version `2020.3.26f1`, and an MIT license visible in the repository page and local checkout. The target repository is `https://github.com/MAZOUNI2025/NeoRumaz`, currently on `main` with one initial README commit and no game files.

## Source architecture

The original game is a 2D sprite-based endless runner in Unity. It uses two scenes (`MainMenu.unity` and `Gameplay.unity`), 14 C# scripts, 31 prefabs, 102 PNGs, 18 MP3 files, 8 animation clips, and 5 animator controllers. The main menu owns `GameManager`, `MainMenuController`, and `CharacterSelectScript`; gameplay owns `GameplayController`, `MapGenerator`, `PlayerController`, `SwipeManager`, `SoundManager`, 21 obstacle-holder groups, scrolling platform holders, UI canvas, camera, and audio.

## Core gameplay logic

The player runs continuously while the camera moves along the X axis. The original controls support keyboard arrows/WASD/space and mobile swipes; one horizontal direction changes between two lanes and an upward gesture jumps. Obstacles are grouped into pre-authored holders and are activated randomly at 0.6-second intervals, with two possible local positions. A collision ends the run unless the player has the T-Rex power-up. Star pickups increase the run currency count, and T-Rex temporarily enables obstacle destruction for seven seconds. Distance becomes score, speed increases at score thresholds 30 and 60, and game over saves the best score and earned stars.

## Progression and persistence

The original stores stars, best score, a nine-element hero-unlock array, and the selected hero index in a binary file under `Application.persistentDataPath`. Hero 0 is initially unlocked; the current source has a testing-only initial balance of 9000 stars, which must not be copied into a production build. Other heroes cost 1000 stars each. The menu supports play, character selection, music toggle, and navigation. The target design should retain this progression loop while replacing unsafe/fragile persistence with robust browser local storage.

## Source weaknesses to improve

The original uses a single active obstacle flag, scene-authored obstacle instances, several singleton references, hard-coded scene lookups, an incorrect swipe condition (`Input.touches.Length < 0`), silent exception swallowing in save/load, duplicated death sound calls, fixed hero pricing, minimal difficulty logic, no pooling guarantee, and no production monetization flow. The browser rebuild should preserve the recognizable loop but use deterministic procedural lanes, pooled/recycled objects, explicit game state, responsive touch/pointer input, robust save migration, and optional monetization placeholders that never block gameplay.

## Delivery constraint

Because the available game-dev pipeline hosts a browser game using Babylon.js inside a WebDev React project, the deliverable will be a playable browser implementation of NeoRumaz with mobile-first responsive controls. It can be Android-ready as a web/PWA-style experience and can document native Android build limitations; it must not falsely claim that a Unity APK or native Android build was produced unless one is actually built and tested.
