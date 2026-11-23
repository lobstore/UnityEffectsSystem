using UnityEngine;

namespace LOGIYGames.EffectSystem
{
    [CreateAssetMenu(fileName = "New EffectData", menuName = "Abilities/Effects/EffectData")]
    public class EffectDefinition : ScriptableObject
    {
        public float Value { get; set; }
        [Min(1)] public int MaxStackSize;
        [Min(1)] public int Levels;
        public GameObject VFX;
        public AudioClip SFX;
        public Sprite Icon;
    }
}
