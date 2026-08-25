// Cairo Night Runner style reminder: monetization remains player-safe, provider-neutral, and free from tracked production credentials.
using System;
using UnityEngine;

namespace NeoRumaz.Runtime
{
    public enum AdPlacement
    {
        RewardedRevive,
        RewardedDailyCredits,
        InterstitialAfterRun
    }

    public enum AdOutcome
    {
        Completed,
        Skipped,
        Unavailable,
        Failed
    }

    public interface IMonetizationProvider
    {
        bool IsReady(AdPlacement placement);
        void Show(AdPlacement placement, Action<AdOutcome> completed);
    }

    [CreateAssetMenu(fileName = "NeoRumazMonetizationConfiguration", menuName = "NeoRumaz/Monetization Configuration")]
    public sealed class MonetizationConfiguration : ScriptableObject
    {
        [Header("Safety policy")]
        public bool EnableRewarded = false;
        public bool EnableInterstitial = false;
        public int MinimumCompletedRunsBeforeInterstitial = 3;
        public float MinimumSecondsBetweenInterstitials = 180f;
        public int RewardedDailyCredits = 50;

        [Header("Provider binding")]
        [Tooltip("Use a provider-owned local configuration or platform dashboard. Do not store production keys or secrets in this asset.")]
        public string ProviderAdapterType = "";
    }

    public sealed class NoOpMonetizationProvider : IMonetizationProvider
    {
        public bool IsReady(AdPlacement placement) { return false; }
        public void Show(AdPlacement placement, Action<AdOutcome> completed)
        {
            if (completed != null) completed(AdOutcome.Unavailable);
        }
    }

    public sealed class MonetizationService
    {
        private readonly IMonetizationProvider provider;
        private readonly MonetizationConfiguration configuration;
        private int completedRuns;
        private float lastInterstitialRealtime;

        public MonetizationService(MonetizationConfiguration configuration, IMonetizationProvider providerOverride = null)
        {
            this.configuration = configuration;
            provider = providerOverride ?? new NoOpMonetizationProvider();
        }

        public bool IsRewardedReady(AdPlacement placement)
        {
            return configuration != null && configuration.EnableRewarded && (placement == AdPlacement.RewardedRevive || placement == AdPlacement.RewardedDailyCredits)
                ? provider.IsReady(placement) : false;
        }

        public void RequestRewarded(AdPlacement placement, Action<AdOutcome> completed)
        {
            if (placement != AdPlacement.RewardedRevive && placement != AdPlacement.RewardedDailyCredits)
            {
                if (completed != null) completed(AdOutcome.Failed);
                return;
            }
            if (configuration == null || !configuration.EnableRewarded)
            {
                if (completed != null) completed(AdOutcome.Unavailable);
                return;
            }
            provider.Show(placement, completed);
        }

        public void TryShowControlledInterstitial(Action<AdOutcome> completed)
        {
            completedRuns += 1;
            bool blockedByPolicy = configuration == null || !configuration.EnableInterstitial || completedRuns < configuration.MinimumCompletedRunsBeforeInterstitial || Time.realtimeSinceStartup - lastInterstitialRealtime < configuration.MinimumSecondsBetweenInterstitials;
            if (blockedByPolicy || !provider.IsReady(AdPlacement.InterstitialAfterRun))
            {
                if (completed != null) completed(AdOutcome.Unavailable);
                return;
            }
            lastInterstitialRealtime = Time.realtimeSinceStartup;
            provider.Show(AdPlacement.InterstitialAfterRun, completed);
        }
    }
}
