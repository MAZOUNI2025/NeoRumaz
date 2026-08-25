// Cairo Night Runner style reminder: editor validation protects the organized Unity source before art replacement or Android packaging.
using System.Collections.Generic;
using NeoRumaz.Runtime;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NeoRumaz.EditorTools
{
    public sealed class NeoRumazProjectValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        [MenuItem("NeoRumaz/Validate Project Source")]
        public static void ValidateFromMenu()
        {
            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();
            Validate(errors, warnings);
            foreach (string warning in warnings) Debug.LogWarning("[NeoRumaz] " + warning);
            if (errors.Count > 0)
            {
                foreach (string error in errors) Debug.LogError("[NeoRumaz] " + error);
                throw new BuildFailedException("NeoRumaz source validation failed with " + errors.Count + " error(s).");
            }
            Debug.Log("[NeoRumaz] Source validation passed with " + warnings.Count + " warning(s).");
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            ValidateFromMenu();
        }

        private static void Validate(List<string> errors, List<string> warnings)
        {
            if (EditorBuildSettings.scenes == null || EditorBuildSettings.scenes.Length == 0 || EditorBuildSettings.scenes[0].path != "Assets/NeoRumaz/Scenes/NeoRumazGame.unity")
                errors.Add("NeoRumazGame.unity must be the first enabled Build Settings scene.");

            GameplayConfiguration gameplay = Resources.Load<GameplayConfiguration>("NeoRumazGameplayConfiguration");
            if (gameplay == null) errors.Add("Missing Resources/NeoRumazGameplayConfiguration.asset.");
            else
            {
                if (gameplay.LaneWidth <= 0f || gameplay.InitialRunSpeed <= 0f || gameplay.MaximumRunSpeed < gameplay.InitialRunSpeed)
                    errors.Add("Gameplay configuration has invalid movement balance values.");
                if (gameplay.ContractTargetCredits < 1 || gameplay.ContractCompletionBonus < 0)
                    errors.Add("Gameplay configuration has invalid Cairo Contract economy values.");
            }

            NeoRumazPrefabCatalog catalog = Resources.Load<NeoRumazPrefabCatalog>("NeoRumazPrefabCatalog");
            if (catalog == null) errors.Add("Missing Resources/NeoRumazPrefabCatalog.asset.");
            else
            {
                if (catalog.RuntimeRoot == null || catalog.Courier == null || catalog.Barrier == null || catalog.Drone == null || catalog.Credit == null || catalog.ScarabShield == null || catalog.NileRush == null)
                    errors.Add("Prefab catalog contains one or more missing references.");
            }

            MonetizationConfiguration monetization = Resources.Load<MonetizationConfiguration>("NeoRumazMonetizationConfiguration");
            if (monetization == null) errors.Add("Missing Resources/NeoRumazMonetizationConfiguration.asset.");
            else if ((monetization.EnableRewarded || monetization.EnableInterstitial) && string.IsNullOrEmpty(monetization.ProviderAdapterType))
                errors.Add("Monetization is enabled but no tested provider adapter type is configured.");
            else if (!monetization.EnableRewarded && !monetization.EnableInterstitial)
                warnings.Add("Monetization is intentionally disabled; this is expected until an Android provider adapter is integrated and tested.");

            if (PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) != "com.neorumaz.runner")
                warnings.Add("Android package identifier differs from com.neorumaz.runner.");
        }
    }
}
