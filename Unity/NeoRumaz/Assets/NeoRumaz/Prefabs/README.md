# Prefab Contract

`NeoRumazRuntime.prefab` is a source-controlled scene entry component. The game still uses `NeoRumazBootstrap` as a runtime safety net, so opening the production scene works even if the prefab is not yet placed by an editor.

Gameplay visuals are currently generated from audited runtime source to keep the source runnable without missing external model links. Final production must replace these generated nodes with authored courier, environment, hazard, pickup, and VFX prefabs while retaining the same gameplay interfaces.
