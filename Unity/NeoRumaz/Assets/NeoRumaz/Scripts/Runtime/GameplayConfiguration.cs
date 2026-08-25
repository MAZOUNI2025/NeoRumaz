// Cairo Night Runner style reminder: small explicit balance object makes the high-speed mobile runner tunable without scattering magic values.
using UnityEngine;

namespace NeoRumaz.Runtime
{
    [CreateAssetMenu(fileName = "NeoRumazGameplayConfiguration", menuName = "NeoRumaz/Gameplay Configuration")]
    public sealed class GameplayConfiguration : ScriptableObject
    {
        [Header("Runner")]
        public float LaneWidth = 2.8f;
        public float InitialRunSpeed = 14f;
        public float MaximumRunSpeed = 28f;
        public float JumpVelocity = 8.2f;
        public float SwipeThresholdPixels = 42f;

        [Header("Power-ups")]
        public float ShieldDurationSeconds = 6f;
        public float NileRushDurationSeconds = 5f;
        public float NileRushSpeedMultiplier = 1.4f;

        [Header("Economy")]
        public int ContractTargetCredits = 6;
        public int ContractCompletionBonus = 3;
    }
}
