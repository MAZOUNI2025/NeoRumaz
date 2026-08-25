# Controlled Android Monetization Integration

NeoRumaz contains a provider-neutral source architecture, not a live advertising integration. `IMonetizationProvider` is the only bridge an Android provider adapter should implement. The shipped `NoOpMonetizationProvider` returns `Unavailable` and sends no network request.

`NeoRumazMonetizationConfiguration.asset` ships with both rewarded and interstitial modes disabled. It has no app identifiers, placement identifiers, API keys, keystore data, or other secrets. A production adapter should obtain required identifiers through the chosen Android provider's supported setup and a local/CI secret mechanism—not through tracked source files.

| Placement | Intended moment | Safety contract |
| --- | --- | --- |
| `RewardedDailyCredits` | Player explicitly presses the optional daily-reward control. | Reward only after the provider returns `Completed`. |
| `RewardedRevive` | Optional post-crash revive, if implemented after device validation. | Never revive after `Skipped`, `Unavailable`, or `Failed`. |
| `InterstitialAfterRun` | Only after a completed run reaches the configured run-count and cooldown gates. | Never interrupt the active run; obey consent, age rating, and provider policy. |

Before enabling either switch, the local Unity project must add a tested adapter, integrate the provider SDK for Android, configure privacy/consent requirements, test on real devices, and verify every reward/error callback. Until then the UI correctly reports that rewarded ads are not configured.
