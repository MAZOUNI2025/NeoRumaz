# NeoRumaz 3D Asset Production Plan

The procedural scene establishes scale, lanes, interaction, lighting intent, and mobile-safe geometry. It is **not** a substitute for finished production models, materials, animation, VFX, or lighting review in Unity.

| Asset group | Current implementation | Production replacement |
| --- | --- | --- |
| Courier | Procedural capsule-based runner with animated limbs | Rigged human courier, run/jump/slide animation set, materials and outfit variants. |
| Road | Pooled procedural highway segments and cyan lane strips | Modular PBR asphalt, lane decals, barriers, guardrails, traffic signage, and puddle material. |
| City | Procedural building blocks and emissive windows | Modular skyline kit, bridge kit, distant skyline impostors, baked or mixed lighting. |
| Hazards | Primitive barriers and drones | Low-poly but silhouette-rich construction kit and animated drone prefabs. |
| Rewards | Procedural credits, shield and boost markers | Branded pickups, particle effects, audio feedback, and clear silhouette language. |
| Background art | Generated horizon reference: `/manus-storage/neorumaz-unity-city-horizon_8b8223d9.jpg` | Import a licensed baked skyline texture or 3D horizon kit after art review. |

The supplied screenshot establishes the expected **composition and quality bar**: a real runner in the lower frame, depth-rich highway, dense urban horizon, pooled props with clear silhouettes, and high-contrast English HUD. It must be matched through final Unity assets and device testing, not through claims based on the current procedural placeholder geometry.
