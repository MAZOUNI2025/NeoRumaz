// Cairo Night Runner style reminder: source-controlled content catalog keeps 3D replacement assets organized without coupling gameplay state to a specific art provider.
using UnityEngine;

namespace NeoRumaz.Runtime
{
    public enum NeoRumazContentKind
    {
        RuntimeRoot,
        Courier,
        Barrier,
        Drone,
        Credit,
        ScarabShield,
        NileRush
    }

    public sealed class NeoRumazPrefabTag : MonoBehaviour
    {
        public NeoRumazContentKind Kind;
        public string StableId;
        [TextArea] public string ReplacementNotes;
    }

    [CreateAssetMenu(fileName = "NeoRumazPrefabCatalog", menuName = "NeoRumaz/Prefab Catalog")]
    public sealed class NeoRumazPrefabCatalog : ScriptableObject
    {
        public GameObject RuntimeRoot;
        public GameObject Courier;
        public GameObject Barrier;
        public GameObject Drone;
        public GameObject Credit;
        public GameObject ScarabShield;
        public GameObject NileRush;
    }
}
