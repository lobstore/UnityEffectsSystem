using UnityEngine;

namespace LOGIYGames.EffectSystem
{
    /// <summary>
    /// Authoring asset that defines the shared configuration of an effect.
    /// A RuntimeEffect receives its own runtime clone of this asset.
    /// </summary>
    [CreateAssetMenu(fileName = "New EffectData", menuName = "Abilities/Effects/EffectData")]
    public class EffectDefinition : ScriptableObject
    {
        [SerializeField, Tooltip("Runtime display value. Each RuntimeEffect works on a clone of this asset.")]
        private float value;

        /// <summary>
        /// Runtime value associated with an effect instance. This value is serialized on the definition
        /// and copied into each runtime clone; changing one active effect does not mutate the source asset.
        /// </summary>
        public float Value
        {
            get => value;
            set => this.value = value;
        }

        /// <summary>Maximum number of simultaneous instances of the same effect type.</summary>
        [Min(1), Tooltip("Maximum simultaneous instances of this effect type on one owner.")]
        public int MaxStackSize = 1;

        /// <summary>Maximum effect level supported by the definition.</summary>
        [Min(1), Tooltip("Maximum level supported by this effect definition.")]
        public int Levels = 1;

        /// <summary>Optional visual effect prefab associated with the effect.</summary>
        public GameObject VFX;

        /// <summary>Optional sound played by the consuming game logic.</summary>
        public AudioClip SFX;

        /// <summary>Optional icon used by effect UI.</summary>
        public Sprite Icon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            MaxStackSize = Mathf.Max(1, MaxStackSize);
            Levels = Mathf.Max(1, Levels);
        }
#endif
    }
}
